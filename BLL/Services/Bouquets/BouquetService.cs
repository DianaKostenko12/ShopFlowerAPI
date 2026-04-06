using AutoMapper;
using BLL.Services.Bouquets.Descriptors;
using BLL.Services.FileStorage;
using DAL.Data.UnitOfWork;
using DAL.Exceptions;
using DAL.Filters;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace BLL.Services.Bouquets
{
    public class BouquetService : IBouquetService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;
        private readonly UserManager<User> _userManager;
        private readonly IFileStorage _fileStorage;

        public BouquetService(IUnitOfWork uow, IMapper mapper, UserManager<User> userManager, IFileStorage fileStorage)
        {
            _uow = uow;
            _mapper = mapper;
            _userManager = userManager;
            _fileStorage = fileStorage;
        }

        public async Task AddBouquetAsync(CreateBouquetDescriptor descriptor, int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            try
            {
                string fileName = null;
                byte[] photoBytes = null;
                string photoContentType = null;

                if (descriptor.WrappingPaperId <= 0)
                {
                    throw new ArgumentException("WrappingPaperId must be provided.");
                }

                var wrappingPaper = await _uow.WrappingPaperRepository.FindAsync(descriptor.WrappingPaperId);
                if (wrappingPaper == null)
                {
                    throw new ArgumentException($"WrappingPaper with ID {descriptor.WrappingPaperId} does not exist.");
                }

                if (string.IsNullOrWhiteSpace(descriptor.Shape))
                {
                    throw new ArgumentException("Shape must be provided.");
                }

                if (descriptor.Photo != null)
                {
                    if (_fileStorage == null)
                    {
                        throw new InvalidOperationException("FileStorage service is not initialized.");
                    }

                    fileName = await _fileStorage.AddFileAsync(descriptor.Photo);
                }
                else if (descriptor.PhotoBytes != null && descriptor.PhotoBytes.Length > 0)
                {
                    photoBytes = descriptor.PhotoBytes;
                    photoContentType = descriptor.PhotoContentType ?? "image/png";
                }
                else
                {
                    throw new ArgumentNullException(nameof(descriptor.Photo), "Photo or PhotoBytes must be provided.");
                }

                var newBouquet = new Bouquet()
                {
                    BouquetName = descriptor.BouquetName,
                    BouquetDescription = descriptor.BouquetDescription,
                    WrappingPaperId = descriptor.WrappingPaperId,
                    Shape = descriptor.Shape,
                    PhotoFileName = fileName,
                    PhotoBytes = photoBytes,
                    PhotoContentType = photoContentType,
                    User = user,
                };
                await _uow.BouquetRepository.AddAsync(newBouquet);

                if (descriptor.Flowers != null && descriptor.Flowers.Any())
                {
                    List<BouquetFlower> newBouquetFlowers = new List<BouquetFlower>();

                    foreach (var flower in descriptor.Flowers)
                    {
                        var flowerExists = await _uow.FlowerRepository.FindAsync(flower.FlowerId);
                        if (flowerExists == null)
                        {
                            throw new ArgumentException($"Flower with ID {flower.FlowerId} does not exist.");
                        }

                        newBouquetFlowers.Add(new BouquetFlower()
                        {
                            Bouquet = newBouquet,
                            FlowerId = flower.FlowerId,
                            FlowerCount = flower.FlowerCount,
                            Role = flower.Role
                        });
                    }

                    await _uow.BouquetFlowerRepository.AddRangeAsync(newBouquetFlowers);
                }

                await _uow.CompleteAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteBouquetAsync(int bouquetId, int userId)
        {
            var bouquetToDelete = await _uow.BouquetRepository.FindAsync(bouquetId);
            if (bouquetToDelete == null)
            {
                throw new KeyNotFoundException($"Bouquet with ID {bouquetId} was not found.");
            }
            User recordAuthor = await _userManager.FindByIdAsync(bouquetToDelete.User.Id.ToString());
            IList<string> recordAuthorRoles = await _userManager.GetRolesAsync(recordAuthor);

            User editor = await _userManager.FindByIdAsync(userId.ToString());
            IList<string> editorRoles = await _userManager.GetRolesAsync(editor);

            if (recordAuthorRoles.Contains(Roles.Admin) && !editorRoles.Contains(Roles.Admin))
            {
                throw new BusinessException(HttpStatusCode.Forbidden, "¬и не маЇте права");
            }

            if (!recordAuthorRoles.Contains(Roles.Admin) && !editorRoles.Contains(Roles.Admin) && recordAuthor.Id != editor.Id)
            {
                throw new BusinessException(HttpStatusCode.Forbidden, "¬и не маЇте права");
            }

            await _uow.BouquetRepository.RemoveAsync(bouquetToDelete);
            await _uow.CompleteAsync();
        }

        public async Task<Bouquet> GetBouquetByIdAsync(int bouquetId)
        {
            return await _uow.BouquetRepository.GetBouquetByIdAsync(bouquetId);
        }

        public async Task<List<Bouquet>> GetBouquetsByUserIdAsync(int userId)
        {
            var bouquets = await _uow.BouquetRepository.GetBouquetsByUserIdAsync(userId);
            if (!bouquets.Any())
            {
                throw new KeyNotFoundException($"No bouquets found for user with ID {userId}.");
            }
            return bouquets;
        }

        public async Task<List<Bouquet>> GetBouquetsByFilterAsync(BouquetFilterView view, int? userId)
        {
            List<Bouquet> bouquets =  await _uow.BouquetRepository.GetBouquetsByFilterAsync(view);
            List<Bouquet> filterBouquets = new List<Bouquet>();
            foreach (var bouquet in bouquets)
            {
                User creator = await _userManager.FindByIdAsync(bouquet.User.Id.ToString());
                IList<string> creatorRoles = await _userManager.GetRolesAsync(creator);
                if (creatorRoles.Contains(Roles.Admin))
                {
                    filterBouquets.Add(bouquet);
                }
            }

            if(userId.HasValue)
            {
                foreach (var bouquet in bouquets)
                {
                    if (await IsUserBouquetOwnerAsync(bouquet.BouquetId, userId.Value))
                    {
                        filterBouquets.Add(bouquet);
                    }
                }
            }

            return filterBouquets.DistinctBy(x => x.BouquetId).ToList();
        }

        public async Task<bool> IsUserBouquetOwnerAsync(int bouquetId, int userId)
        {
            var bouquet = await _uow.BouquetRepository.GetBouquetByIdAsync(bouquetId);

            return bouquet != null && bouquet.User.Id == userId;
        }
    }
}
