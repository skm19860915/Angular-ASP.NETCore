using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using b = BL.BusinessObjects;
using DAL.CustomerExceptions;
using BL.CustomExceptions;
using Models.Enums;
using Models.MultiMedia;

namespace BL
{
    public interface IMovieManager
    {
        ITagRepo<MovieTag> _movieTagRepo { get; set; }
        void AddTagsToMovies(List<MovieTag> tags, int movieId, Guid createdUserGuid);
        void Archive(int id, Guid createdUserGuid);
        int CreateNewMovie(string url, string name, Guid userToken, List<MovieTag> movieTags);
        List<b.MultiMedia.Movie> GetAllMovies(Guid userToken);
        void HardDelete(int movieId, Guid userToken);
        void UnArchive(int id, Guid createdUserGuid);
        void UpdateMovie(int id, string url, string name, Guid userToken, List<MovieTag> movieTags);
    }

    public class MovieManager : IMovieManager
    {
        private IMultimediaRepo _mmRepo { get; set; }
        public ITagRepo<MovieTag> _movieTagRepo { get; set; }
        private IOrganizationRepo _orgRepo { get; set; }
        private List<OrganizationRoleEnum> _userRoles { get; set; }
        private IUserRepo _userRepo { get; set; }
        public MovieManager(IMultimediaRepo mmrepo, ITagRepo<MovieTag> mmtagRepo, IOrganizationRepo orgRepo, IUserRepo userRepo)
        {
            _mmRepo = mmrepo;
            _movieTagRepo = mmtagRepo;
            _orgRepo = orgRepo;
            _userRepo = userRepo;
            _movieTagRepo.InitializeTagRepo(TagEnum.Movie);
        }

        private void GenerateUserRoles(Guid userToken)
        {
            _userRoles = _orgRepo.GetUserRoles(userToken);
        }
        public void HardDelete(int movieId, Guid userToken)
        {
            GenerateUserRoles(userToken);
            if (!(_userRoles.Contains(OrganizationRoleEnum.ArchiveMovies) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To DELETE Movies");
            }
            var targetMovie = _mmRepo.GetMovie(movieId, userToken);
            if (!targetMovie.CanModify) throw new ApplicationException("This Video Is In Use, And Cannot Be Deleted");

            _movieTagRepo.DeleteAssociatedTags(movieId);
            _mmRepo.DeleteMovie(movieId);
        }
        public int CreateNewMovie(string url, string name, Guid userToken, List<MovieTag> movieTags)
        {
            var tempVideo = string.Empty;
            GenerateUserRoles(userToken);
            if (url.Contains("https://youtu.be/"))
            {
                tempVideo = url.Replace("https://youtu.be/", "https://www.youtube.com/embed/");
            }
            else if (url.Contains("https://vimeo.com/"))
            {
                tempVideo = url.Replace("https://vimeo.com/", "https://player.vimeo.com/video/");
            }
            url = tempVideo;
            if (!(_userRoles.Contains(OrganizationRoleEnum.CreateMovies) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Create Videos");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ItemValidationError("Video Name Is Invalid. Please Change the name");
            }
            var newId = 0;
            try
            {
                if (!_userRepo.Get(userToken).IsCoach) throw new ApplicationException("Only Coaches Can Create Videos");
            }
            catch (Exception)
            {
                throw new ApplicationException("The account You are using has an issue; Please LOGOUT and LOG back in");
            }
            try
            {
                newId = _mmRepo.CreateMovie(url, name, userToken);
            }
            catch (DuplicateKeyException dup)
            {
                throw new ItemAlreadyExistsException("A Video With This Name Already Exists", dup);
            }
            AddTagsToMovies(movieTags, newId, userToken);
            return newId;
        }
        public void UpdateMovie(int id, string url, string name, Guid userToken, List<MovieTag> movieTags)
        {
            var tempVideo = string.Empty;
            GenerateUserRoles(userToken);
            if (url.Contains("https://youtu.be/"))
            {
                tempVideo = url.Replace("https://youtu.be/", "https://www.youtube.com/embed/");
            }
            else if (url.Contains("https://vimeo.com/"))
            {
                tempVideo = url.Replace("https://vimeo.com/", "https://player.vimeo.com/video/");
            }

            if (!(_userRoles.Contains(OrganizationRoleEnum.ModifyMovies) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Modify Videos");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ItemValidationError("Video Name Is Invalid. Please Change the name");
            }
            var targetMovie = _mmRepo.GetMovie(id, userToken);
            try
            {
                if (!_userRepo.Get(userToken).IsCoach) throw new ApplicationException("Only Coaches Can Create Videos");
            }
            catch (Exception)
            {
                throw new ApplicationException("The account You are using has an issue; Please LOGOUT and LOG back in");
            }
            _mmRepo.UpdateMovie(url, name, id, userToken);
            AddTagsToMovies(movieTags, targetMovie.Id, userToken);
        }
        public void UnArchive(int id, Guid createdUserGuid)
        {
            GenerateUserRoles(createdUserGuid);
            if (!(_userRoles.Contains(OrganizationRoleEnum.ArchiveMovies) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To UnArchive Videos");
            }
            _mmRepo.UnArchiveMovie(id, createdUserGuid);
        }
        public void Archive(int id, Guid createdUserGuid)
        {
            GenerateUserRoles(createdUserGuid);
            if (!(_userRoles.Contains(OrganizationRoleEnum.ArchiveMovies) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Archive Videos");
            }
            _mmRepo.ArchiveMovie(id, createdUserGuid);
        }
        public List<b.MultiMedia.Movie> GetAllMovies(Guid userToken)
        {

            var allMappings = _mmRepo.GetAllMovieTagMappings(userToken);
            var allMovies = _mmRepo.GetAllMovies(userToken);

            var ret = new List<b.MultiMedia.Movie>();

            foreach (var m in allMovies)
            {
                var mapping = allMappings.FirstOrDefault(x => x.MovieId == m.Id);
                ret.Add(new b.MultiMedia.Movie()
                {
                    Id = m.Id,
                    URL = m.URL,
                    Name = m.Name,
                    Tags = mapping == null ? new List<b.Tag>() : mapping.Tags.Select(x => new b.Tag() { Id = x.Id, Name = x.Name }).ToList(),
                    CanModify = m.CanModify,
                    IsDeleted = m.IsDeleted
                });
            }
            return ret;
        }
        public void AddTagsToMovies(List<MovieTag> tags, int movieId, Guid createdUserGuid)
        {
            GenerateUserRoles(createdUserGuid);
            if (!(_userRoles.Contains(OrganizationRoleEnum.CreateMovies) || _userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.ModifyMovies)))
            {
                throw new ApplicationException("User Does Not Have Rights To Modify Exercises");
            }

            var targetMovie = _mmRepo.GetMovie(movieId, createdUserGuid);
            if (targetMovie == null) return;

            _movieTagRepo.DeleteAssociatedTags(movieId);
            _movieTagRepo.AddAssociatedTags(tags, movieId);
        }
    }
}
