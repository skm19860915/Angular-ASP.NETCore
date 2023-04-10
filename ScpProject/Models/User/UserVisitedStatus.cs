using System;

namespace Models.User
{
    public class UserVisitedStatus
    {
        public UserVisitedStatus(Boolean visitedExercise, Boolean visitedPrograms, Boolean vistedRosters, Boolean visitedSetsReps, Boolean visitedSurveys, Boolean visitedMetrics, Boolean visitedCoachRoster, Boolean visitedProgramBuilder)
        {
            VisitedExercise = visitedExercise;
            VisitedPrograms = visitedPrograms;
            VistedRosters = vistedRosters;
            VisitedSetsReps = visitedSetsReps;
            VisitedSurveys = visitedSurveys;
            VisitedMetrics = visitedMetrics;
            VisitedCoachRoster = visitedCoachRoster;
            VisitedProgramBuilder = visitedProgramBuilder;
        }

        public Boolean VisitedExercise { get; set; }
        public Boolean VisitedPrograms { get; set; }
        public Boolean VistedRosters { get; set; }
        public Boolean VisitedSetsReps { get; set; }
        public Boolean VisitedSurveys { get; set; }
        public Boolean VisitedMetrics { get; set; }
        public Boolean VisitedCoachRoster { get; set; }
        public Boolean VisitedProgramBuilder { get; set; }
    }
}
