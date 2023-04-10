using DAL.Repositories;
using System;
using System.Collections.Generic;
using Models.Tag;
using System.Data.Linq;
using Models.Enums;

namespace BL
{
    public interface ITagManager<T> where T : Tag
    {

         int CreateTag(TagEnum tagType, string name, string notes, Guid userToken);
        List<T> GetAllTags(TagEnum targetTagType, Guid userToken);
    }

    public class TagManager<T> : ITagManager<T> where T : Tag
    {
        private ITagRepo<T>_tagRepo { get; set; }
        private IUserRepo _userRepo { get; set; }
        private IWeightRoomRepo _weightRoomRepo { get; set; }
        public TagManager(IUserRepo userRepo, ITagRepo<T> tagRepo, IWeightRoomRepo weightRoomRepo)
        {
            
            _userRepo = userRepo;
            _weightRoomRepo = weightRoomRepo;
            _tagRepo = tagRepo;
        }


        public int CreateTag(TagEnum tagType,string name, string notes, Guid userToken)
        {
            var ret = 0;
            _tagRepo.InitializeTagRepo(tagType);
            try
            {
                ret = _tagRepo.CreateTag(name,notes, _userRepo.Get(userToken).OrganizationId);
            }
            catch (DuplicateKeyException dup)//if it already exsits return the pre-existing ID
            {
                ret = _tagRepo.GetTag(name, _userRepo.Get(userToken).OrganizationId);
            }
            catch (System.Data.SqlClient.SqlException dup)
            {
                if (dup.Message.Contains("Cannot insert duplicate key row in object"))
                {
                    ret = _tagRepo.GetTag(name, _userRepo.Get(userToken).OrganizationId);
                }
                else
                {//todo:add logging and notification
                    throw;
                }

            }
            return ret;
        }
        public List<T> GetAllTags(TagEnum targetTagType, Guid userToken)
        {
            var weightRoomUser = _weightRoomRepo.Get(userToken);
            var targetUser = _userRepo.Get(userToken);
            //the weightroom view needs access to grabbing all tags.
            _tagRepo.InitializeTagRepo(targetTagType);
            return _tagRepo.GetAllTags(targetUser.OrganizationId == 0 ? weightRoomUser.OrganizationId : targetUser.OrganizationId);
        }
    }
}
