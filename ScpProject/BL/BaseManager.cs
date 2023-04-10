using BL.CustomExceptions;
using DAL.Repositories;
using Models.Enums;
using System;
using System.Collections.Generic;

namespace BL
{
    public class BaseManager
    {
        public string ConnectionString { get; set; }
        public UserRepo _userRepo { get; set; }
        public OrganizationRepo _orgRepo { get; set; }
        public WeightRoomRepo _weightRoomRepo { get; set; }
        public List<OrganizationRoleEnum> _userRoles { get; set; }
        public LogRepo _logRepo { get; set; }
        public BaseManager(string connectionString, Guid userToken)
        {

            _userRepo = new UserRepo(connectionString);
            _orgRepo = new OrganizationRepo(connectionString);
            _weightRoomRepo = new WeightRoomRepo(connectionString);
            _logRepo = new LogRepo(connectionString);
            var targetOrg = _orgRepo.GetOrg(_userRepo.Get(userToken).OrganizationId);
            var targetWeightRoomAccount = _weightRoomRepo.Get(userToken);
            ConnectionString = connectionString;
            //if they are weight room account we are letting them use this. Because when they use their normal account to do anything we are checking it
            if (targetWeightRoomAccount != null)
            {
                return;
            }

            if (targetOrg == null || !targetOrg.IsCustomer)//order matters, INvalidCustomer must be thrown before BadAccount
            {
                throw new InvalidCustomerException();
            }
            if (targetOrg.StripeFailedToProcess)
            {
                throw new StripeFailedToProcessException();
            }
            _userRoles = _orgRepo.GetUserRoles(userToken);
        }
        public BaseManager(string connectionString)
        {
            _userRepo = new UserRepo(connectionString);
            _orgRepo = new OrganizationRepo(connectionString);
            _weightRoomRepo = new WeightRoomRepo(connectionString);
            ConnectionString = connectionString;
        }

        public List<OrganizationRoleEnum> GetRoles(Guid userToken)
        {
            return _orgRepo.GetUserRoles(userToken);
        }

    }
}
