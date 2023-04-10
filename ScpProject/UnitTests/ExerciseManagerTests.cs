using BL;
using BL.CustomExceptions;
using DAL.CustomerExceptions;
using DAL.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Exercise;
using Models.User;
using Moq;
using System;

namespace UnitTests
{
    [TestClass]
    public class ExerciseManagerTests
    {
        private Mock<IUserRepo> _userRepoMock;
        private Mock<IExerciseRepo> _exerciseRepoMock;
        private Mock<IOrganizationRepo> _orgRepoMock;
        private Mock<ITagRepo<ExerciseTag>> _exerciseTagRepoMock;
        private readonly ExerciseManager testObject;
        private Exercise _testExercise;
        private Guid _userToken;
        private User _testUser;
        private int _organizationId;
        private System.Collections.Generic.List<ExerciseTag> _testTags;

        public ExerciseManagerTests()
        {
            _orgRepoMock = new Mock<IOrganizationRepo>();
            _exerciseRepoMock = new Mock<IExerciseRepo>();
            _userRepoMock = new Mock<IUserRepo>();
            _exerciseTagRepoMock = new Mock<ITagRepo<ExerciseTag>>();
            testObject = new ExerciseManager(_exerciseRepoMock.Object, _userRepoMock.Object, _orgRepoMock.Object, _exerciseTagRepoMock.Object);
            _userToken = Guid.NewGuid();
            _testExercise = new Exercise() { CanModify = true, Id = 0 };
            _organizationId = 0;
            _testUser = new User() { OrganizationId = _organizationId };
            _testTags = new System.Collections.Generic.List<ExerciseTag> { };
        }

        [TestMethod]
        public void Hard_Delete_User_Lacks_Permissions()
        {
            _orgRepoMock.Setup(x => x.GetUserRoles(_userToken))
                .Returns(new System.Collections.Generic.List<Models.Enums.OrganizationRoleEnum>() { });

            _userRepoMock.Setup(x => x.Get(_userToken)).Returns(_testUser);
            _exerciseRepoMock.Setup(x => x.GetCreatedUser(_testExercise.Id)).Returns(_testUser);

            //  testObject.GenerateUserRoles(_userToken);

            // Actual Tests
            Assert.ThrowsException<ApplicationException>(() =>
            {
                // Code to Test
                testObject.HardDelete(0, _userToken);
            });
        }


        [TestMethod]
        [DataRow(Models.Enums.OrganizationRoleEnum.Admin)]
        [DataRow(Models.Enums.OrganizationRoleEnum.ArchiveExercises)]
        public void Hard_Delete_Cannot_Modify(Models.Enums.OrganizationRoleEnum role)
        {
            // Mock Setup
            _orgRepoMock.Setup(x => x.GetUserRoles(_userToken))
                .Returns(new System.Collections.Generic.List<Models.Enums.OrganizationRoleEnum>() { role });

            _userRepoMock.Setup(x => x.Get(_userToken)).Returns(_testUser);
            _exerciseRepoMock.Setup(x => x.GetCreatedUser(_testExercise.Id)).Returns(_testUser);

            _exerciseRepoMock.Setup(x => x.GetExercise(_testExercise.Id, _userToken)).Returns(_testExercise);

            _exerciseTagRepoMock.Setup(x => x.DeleteAssociatedTags(_testExercise.Id));
            _exerciseRepoMock.Setup(x => x.HardDelete(_testExercise.Id));

            _testExercise.CanModify = false;

            //  testObject.GenerateUserRoles(_userToken);

            // Actual Tests
            Assert.ThrowsException<ApplicationException>(() =>
            {
                // Code to Test
                testObject.HardDelete(_testExercise.Id, _userToken);
            });
        }

        [TestMethod]
        [DataRow(Models.Enums.OrganizationRoleEnum.Admin)]
        [DataRow(Models.Enums.OrganizationRoleEnum.ArchiveExercises)]
        public void Hard_Delete_Happy_Path(Models.Enums.OrganizationRoleEnum role)
        {

            // Mock Setup
            _orgRepoMock.Setup(x => x.GetUserRoles(_userToken))
                .Returns(new System.Collections.Generic.List<Models.Enums.OrganizationRoleEnum>() { role });

            _userRepoMock.Setup(x => x.Get(_userToken)).Returns(_testUser);
            _exerciseRepoMock.Setup(x => x.GetCreatedUser(_testExercise.Id)).Returns(_testUser);

            _exerciseRepoMock.Setup(x => x.GetExercise(_testExercise.Id, _userToken)).Returns(_testExercise);

            _exerciseTagRepoMock.Setup(x => x.DeleteAssociatedTags(_testExercise.Id));
            _exerciseRepoMock.Setup(x => x.HardDelete(_testExercise.Id));

            // Code to Test
            // testObject.GenerateUserRoles(_userToken);
            testObject.HardDelete(_testExercise.Id, _userToken);

            // Actual Tests
            _exerciseTagRepoMock.Verify(x => x.DeleteAssociatedTags(_testExercise.Id), Times.Once);
            _exerciseRepoMock.Verify(x => x.HardDelete(_testExercise.Id), Times.Once);

        }

        [TestMethod]
        public void Hard_Delete_User_Not_In_Organization()
        {
            _userRepoMock.Setup(x => x.Get(_userToken)).Returns(_testUser);
            _exerciseRepoMock.Setup(x => x.GetCreatedUser(_testExercise.Id)).Returns(new Models.User.User() { OrganizationId = ++_organizationId });

            Assert.ThrowsException<ApplicationException>(() =>
            {
                testObject.HardDelete(_testExercise.Id, _userToken);
            });
        }

        [TestMethod]
        public void Create_New_Exercise_User_Lacks_Permissions()
        {
            _orgRepoMock.Setup(x => x.GetUserRoles(_userToken))
                .Returns(new System.Collections.Generic.List<Models.Enums.OrganizationRoleEnum>() { });

            _userRepoMock.Setup(x => x.Get(_userToken)).Returns(_testUser);

            Assert.ThrowsException<ApplicationException>(() =>
            {
                testObject.CreateNewExercise("", "", _testTags, _userToken, 0.0, 0, "", _userToken);
            });
        }

        [TestMethod]
        [DataRow(Models.Enums.OrganizationRoleEnum.Admin)]
        [DataRow(Models.Enums.OrganizationRoleEnum.CreateExercises)]
        public void Create_New_Exercise_Empty_Exercise_Name(Models.Enums.OrganizationRoleEnum role)
        {
            _orgRepoMock.Setup(x => x.GetUserRoles(_userToken))
                .Returns(new System.Collections.Generic.List<Models.Enums.OrganizationRoleEnum>() { role });

            _userRepoMock.Setup(x => x.Get(_userToken)).Returns(_testUser);

            Assert.ThrowsException<ItemValidationError>(() =>
            {
                testObject.CreateNewExercise("", "", _testTags, _userToken, 0.0, 0, "", _userToken);
            });

        }

        [TestMethod]
        [DataRow(Models.Enums.OrganizationRoleEnum.Admin)]
        [DataRow(Models.Enums.OrganizationRoleEnum.CreateExercises)]
        public void Create_New_Exercise_Not_Coach(Models.Enums.OrganizationRoleEnum role)
        {
            _orgRepoMock.Setup(x => x.GetUserRoles(_userToken))
                .Returns(new System.Collections.Generic.List<Models.Enums.OrganizationRoleEnum>() { role });

            _testUser.IsCoach = false;

            _userRepoMock.Setup(x => x.Get(_userToken)).Returns(_testUser);


            Assert.ThrowsException<ApplicationException>(() =>
            {
                testObject.CreateNewExercise("", "test", _testTags, _userToken, 0.0, 0, "", _userToken);
            });
        }

        [TestMethod]
        [DataRow(Models.Enums.OrganizationRoleEnum.Admin)]
        [DataRow(Models.Enums.OrganizationRoleEnum.CreateExercises)]
        public void Create_New_Exercise_Duplicate_Name(Models.Enums.OrganizationRoleEnum role)
        {
            _orgRepoMock.Setup(x => x.GetUserRoles(_userToken))
                .Returns(new System.Collections.Generic.List<Models.Enums.OrganizationRoleEnum>() { role });

            _testUser.IsCoach = true;

            _userRepoMock.Setup(x => x.Get(_userToken)).Returns(_testUser);

            _exerciseRepoMock.Setup(x => x.CreateExericse(It.IsAny<Exercise>(), _userToken)).Throws(new DuplicateKeyException());

            Assert.ThrowsException<ItemAlreadyExistsException>(() =>
            {
                testObject.CreateNewExercise("", "test", _testTags, _userToken, 0, 0, "", _userToken);
            });

        }

        [TestMethod]
        [DataRow(Models.Enums.OrganizationRoleEnum.Admin)]
        [DataRow(Models.Enums.OrganizationRoleEnum.CreateExercises)]
        public void Create_New_Exercise_Happy_Path(Models.Enums.OrganizationRoleEnum role)
        {
            _orgRepoMock.Setup(x => x.GetUserRoles(_userToken))
                .Returns(new System.Collections.Generic.List<Models.Enums.OrganizationRoleEnum>() { role });

            _testUser.IsCoach = true;

            _userRepoMock.Setup(x => x.Get(_userToken)).Returns(_testUser);

            _exerciseRepoMock.Setup(x => x.CreateExericse(It.IsAny<Exercise>(), _userToken)).Returns(1);

            var return_val = testObject.CreateNewExercise("", "test", _testTags, _userToken, 0, 0, "", _userToken);

            Assert.IsInstanceOfType(return_val, typeof(int));
            Assert.IsNotNull(return_val);
            Assert.AreEqual(return_val, 1);

        }

        [TestMethod]
        public void Update_Exercise_User_Lacks_Permissions()
        {

            _orgRepoMock.Setup(x => x.GetUserRoles(_userToken))
                .Returns(new System.Collections.Generic.List<Models.Enums.OrganizationRoleEnum>() { });

            _userRepoMock.Setup(x => x.Get(_userToken)).Returns(_testUser);

            Assert.ThrowsException<ApplicationException>(() =>
            {
                testObject.UpdateExercise(_testExercise.Id, "", _testExercise.Name, _testTags, _userToken, 0, 0, "", _userToken);
            });

        }

        [TestMethod]
        [DataRow(Models.Enums.OrganizationRoleEnum.Admin)]
        [DataRow(Models.Enums.OrganizationRoleEnum.ModifyExercises)]
        public void Update_Exercise_Cannot_Modify(Models.Enums.OrganizationRoleEnum role)
        {
            _orgRepoMock.Setup(x => x.GetUserRoles(_userToken))
                .Returns(new System.Collections.Generic.List<Models.Enums.OrganizationRoleEnum>() { role });

            _userRepoMock.Setup(x => x.Get(_userToken)).Returns(_testUser);

            _testExercise.CanModify = false;

            _exerciseRepoMock.Setup(x => x.GetExercise(_testExercise.Id, _userToken)).Returns(_testExercise);

            testObject.UpdateExercise(_testExercise.Id, "", _testExercise.Name, _testTags, _userToken, 0, 0, "", _userToken);

            _exerciseRepoMock.Verify(x => x.UpdateExercise(It.IsAny<Exercise>(), _userToken), Times.Once);
        }

        [TestMethod]
        [DataRow(Models.Enums.OrganizationRoleEnum.Admin)]
        [DataRow(Models.Enums.OrganizationRoleEnum.ModifyExercises)]
        public void Update_Exercise_Can_Modify(Models.Enums.OrganizationRoleEnum role)
        {
            _orgRepoMock.Setup(x => x.GetUserRoles(_userToken))
                .Returns(new System.Collections.Generic.List<Models.Enums.OrganizationRoleEnum>() { role });

            _userRepoMock.Setup(x => x.Get(_userToken)).Returns(_testUser);


            _exerciseRepoMock.Setup(x => x.GetExercise(_testExercise.Id, _userToken)).Returns(_testExercise);

            testObject.UpdateExercise(_testExercise.Id, "", _testExercise.Name, _testTags, _userToken, 0, 0, "", _userToken);

            _exerciseRepoMock.Verify(x => x.UpdateExercise(It.IsAny<Exercise>(), _userToken), Times.Once);
        }

        [TestMethod]
        public void UnArchive_User_Lacks_Permissions()
        {
            _orgRepoMock.Setup(x => x.GetUserRoles(_userToken)).Returns(new System.Collections.Generic.List<Models.Enums.OrganizationRoleEnum>() { });

            _userRepoMock.Setup(x => x.Get(_userToken)).Returns(_testUser);

            Assert.ThrowsException<ApplicationException>(() =>
            {
                testObject.UnArchive(_testExercise.Id, _userToken);
            });
        }

        [TestMethod]
        [DataRow(Models.Enums.OrganizationRoleEnum.Admin)]
        [DataRow(Models.Enums.OrganizationRoleEnum.ArchiveExercises)]
        public void UnArchive_Happy_Path(Models.Enums.OrganizationRoleEnum role)
        {
            _orgRepoMock.Setup(x => x.GetUserRoles(_userToken)).Returns(new System.Collections.Generic.List<Models.Enums.OrganizationRoleEnum>() { role });

            _userRepoMock.Setup(x => x.Get(_userToken)).Returns(_testUser);

            testObject.UnArchive(_testExercise.Id, _userToken);

            _exerciseRepoMock.Verify(x => x.UnArchive(_testExercise.Id, _userToken), Times.Once);
        }

        [TestMethod]
        public void Archive_User_Lacks_Permissions()
        {
            _orgRepoMock.Setup(x => x.GetUserRoles(_userToken)).Returns(new System.Collections.Generic.List<Models.Enums.OrganizationRoleEnum>() { });

            _userRepoMock.Setup(x => x.Get(_userToken)).Returns(_testUser);

            Assert.ThrowsException<ApplicationException>(() =>
            {
                testObject.Archive(_testExercise.Id, _userToken);
            });
        }

        [TestMethod]
        [DataRow(Models.Enums.OrganizationRoleEnum.Admin)]
        [DataRow(Models.Enums.OrganizationRoleEnum.ArchiveExercises)]
        public void Archive_Happy_Path(Models.Enums.OrganizationRoleEnum role)
        {
            _orgRepoMock.Setup(x => x.GetUserRoles(_userToken)).Returns(new System.Collections.Generic.List<Models.Enums.OrganizationRoleEnum>() { role });

            _userRepoMock.Setup(x => x.Get(_userToken)).Returns(_testUser);

            testObject.Archive(_testExercise.Id, _userToken);

            _exerciseRepoMock.Verify(x => x.Archive(_testExercise.Id, _userToken), Times.Once);
        }

        [TestMethod]
        public void Duplicate_User_Lacks_Permissions()
        {
            _orgRepoMock.Setup(x => x.GetUserRoles(_userToken)).Returns(new System.Collections.Generic.List<Models.Enums.OrganizationRoleEnum>() { });

            _userRepoMock.Setup(x => x.Get(_userToken)).Returns(_testUser);

            Assert.ThrowsException<ApplicationException>(() =>
            {
                testObject.Duplicate(_testExercise.Id, _userToken);
            });
        }

        [TestMethod]
        [DataRow(Models.Enums.OrganizationRoleEnum.Admin)]
        [DataRow(Models.Enums.OrganizationRoleEnum.CreateExercises)]
        public void Duplicate_Happy_Path(Models.Enums.OrganizationRoleEnum role)
        {
            _orgRepoMock.Setup(x => x.GetUserRoles(_userToken)).Returns(new System.Collections.Generic.List<Models.Enums.OrganizationRoleEnum>() { role });

            _userRepoMock.Setup(x => x.Get(_userToken)).Returns(_testUser);

            testObject.Duplicate(_testExercise.Id, _userToken);

            _exerciseRepoMock.Verify(x => x.DuplicateExercise(_testExercise.Id, _userToken), Times.Once);
        }

        [TestMethod]
        public void Add_Tags_To_Exercise_User_Lacks_Permissions()
        {
            _orgRepoMock.Setup(x => x.GetUserRoles(_userToken)).Returns(new System.Collections.Generic.List<Models.Enums.OrganizationRoleEnum>() { });

            _userRepoMock.Setup(x => x.Get(_userToken)).Returns(_testUser);

            Assert.ThrowsException<ApplicationException>(() =>
            {
                testObject.AddTagsToExercise(_testTags, _testExercise.Id, _userToken);
            });

        }

        [TestMethod]
        [DataRow(Models.Enums.OrganizationRoleEnum.Admin)]
        [DataRow(Models.Enums.OrganizationRoleEnum.CreateExercises)]
        [DataRow(Models.Enums.OrganizationRoleEnum.ModifyExercises)]
        public void Add_Tags_To_Exercise_Happy_Path(Models.Enums.OrganizationRoleEnum role)
        {
            _orgRepoMock.Setup(x => x.GetUserRoles(_userToken)).Returns(new System.Collections.Generic.List<Models.Enums.OrganizationRoleEnum>() { role });

            _userRepoMock.Setup(x => x.Get(_userToken)).Returns(_testUser);

            _exerciseRepoMock.Setup(x => x.GetExercise(_testExercise.Id, _userToken)).Returns(_testExercise);

            testObject.AddTagsToExercise(_testTags, _testExercise.Id, _userToken);

            _exerciseTagRepoMock.Verify(x => x.AddAssociatedTags(_testTags, _testExercise.Id), Times.Once);
            _exerciseTagRepoMock.Verify(x => x.DeleteAssociatedTags(_testExercise.Id), Times.Once);
        }

        [TestMethod]
        public void Get_All_Exercises_No_Exercises()
        {
            _exerciseRepoMock.Setup(x => x.GetAllExerciseTagMappings(_userToken)).Returns(new System.Collections.Generic.List<DAL.DTOs.ExerciseWithTagsDTO>() { });

            _exerciseRepoMock.Setup(x => x.GetAllExercises(_userToken)).Returns(new System.Collections.Generic.List<DAL.DTOs.Exercises.ExerciseDTO>() { });

            var return_val = testObject.GetAllExercises(_userToken);

            Assert.AreEqual(return_val.Count, 0);
        }

        [TestMethod]
        public void Get_All_Exercises_No_Tag_Mappings()
        {
            _exerciseRepoMock.Setup(x => x.GetAllExerciseTagMappings(_userToken)).Returns(new System.Collections.Generic.List<DAL.DTOs.ExerciseWithTagsDTO>() { });

            _exerciseRepoMock.Setup(x => x.GetAllExercises(_userToken)).
                Returns(new System.Collections.Generic.List<DAL.DTOs.Exercises.ExerciseDTO>()
                { 
                    new DAL.DTOs.Exercises.ExerciseDTO()
                    {
                        Id=_testExercise.Id,
                        Name=_testExercise.Name,
                        Notes=_testExercise.Notes,
                        CreatedUserId=_testExercise.CreatedUserId,
                        IsDeleted=_testExercise.IsDeleted,
                        CanModify=_testExercise.CanModify,
                        Percent=_testExercise.Percent,
                        PercentMetricCalculationId=_testExercise.PercentMetricCalculationId,
                        VideoURL=_testExercise.VideoURL,
                        OrganizationId=_testExercise.OrganizationId,
                    }
                });

            var return_val = testObject.GetAllExercises(_userToken);

            Assert.AreEqual(return_val.Count, 1);
            Assert.AreEqual(return_val[0].Id, _testExercise.Id);
            Assert.AreEqual(return_val[0].Tags.Count, 0);


        }

        [TestMethod]
        public void Get_All_Exercises_Tag_Mappings()
        {
            _exerciseRepoMock.Setup(x => x.GetAllExerciseTagMappings(_userToken))
                .Returns(new System.Collections.Generic.List<DAL.DTOs.ExerciseWithTagsDTO>() {
                    new DAL.DTOs.ExerciseWithTagsDTO()
                    {
                        ExerciseId=_testExercise.Id,
                        Tags=new System.Collections.Generic.List<DAL.DTOs.TagDTO>() { 
                            new DAL.DTOs.TagDTO() { }
                        }
                    }
                });


            _exerciseRepoMock.Setup(x => x.GetAllExercises(_userToken))
                .Returns(new System.Collections.Generic.List<DAL.DTOs.Exercises.ExerciseDTO>()
                { 
                    new DAL.DTOs.Exercises.ExerciseDTO()
                    {
                        Id=_testExercise.Id,
                        Name=_testExercise.Name,
                        Notes=_testExercise.Notes,
                        CreatedUserId=_testExercise.CreatedUserId,
                        IsDeleted=_testExercise.IsDeleted,
                        CanModify=_testExercise.CanModify,
                        Percent=_testExercise.Percent,
                        PercentMetricCalculationId=_testExercise.PercentMetricCalculationId,
                        VideoURL=_testExercise.VideoURL,
                        OrganizationId=_testExercise.OrganizationId,
                    }
                });

            var return_val = testObject.GetAllExercises(_userToken);

            Assert.AreEqual(return_val.Count, 1);
            Assert.AreEqual(return_val[0].Id, _testExercise.Id);
            Assert.AreEqual(return_val[0].Tags.Count, 1);
        }
    }
}
