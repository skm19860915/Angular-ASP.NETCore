import { NgModule, Component } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './Components/Login/Login.component';
import { RegisterComponent } from './Components/Register/Register.component';
import { ExerciseComponent } from './Components/Exercise/Exercise.component';
import { SetsAndRepsComponent } from './Components/sets-and-reps/sets-and-reps.component';
import { SurveyComponent } from './Components/survey/survey.component';
import { RosterComponent } from './Components/roster/roster.component';
import { MultimediaComponent } from './Components/multimedia/multimedia.component';
import { ProgramBuilderComponent } from './Components/program-builder/program-builder.component';
import { MetricComponent } from './Components/metric/metric.component';
import { ProgramComponent } from './Components/program/program.component';
import { AuthGuard } from './Guard/auth.guard';
import { HomeComponent } from './Components/home/home.component';
import { AthleteHomeComponent } from './Components/athlete-home/athlete-home.component';
import { AthleteWorkoutComponent } from './Components/athlete-workout/athlete-workout.component';
import { AthleteEmailVerificationComponent } from './Components/athlete-email-verification/athlete-email-verification.component';
import { AccountSettingsComponent } from './Components/account-settings/account-settings.component';
import { CoachesRosterComponent } from './Components/coaches-roster/coaches-roster.component';
import { AssistantCoachEmailVerificationComponent } from './Components/assistant-coach-email-verification/assistant-coach-email-verification.component';
import { OneTimeComponent } from './Components/one-time/one-time.component';
import { WhiteboardComponent } from './Components/whiteboard/whiteboard.component';
import { ResetPasswordComponent } from './Components/reset-password/reset-password.component';
import { CreateAthleteComponent } from './Components/create-athlete/create-athlete.component';
import { WeightRoomComponent } from './Components/weight-room-components/weight-room/weight-room.component';
import { StripeDataComponent } from './Components/stripe-data/stripe-data.component';
import { ChatComponent } from './Components/chat/chat.component';
import { NotificationsComponent } from './Components/notifications/notifications.component';
import { AthleteMetricComponent } from './Components/athlete-metric/athlete-metric.component';
import { AthleteSurveyComponent } from './Components/athlete-survey/athlete-survey.component';
import { AthleteBioComponent } from './Components/athlete-bio/athlete-bio.component';
import { AthletepastWorkoutComponent } from './Components/athletepast-workout/athletepast-workout.component';
import{ NewRegComponent} from './Components/newRegistration/new-reg/new-reg.component'
import { SnapShotProgramEditorComponent } from './Components/snap-shot-program-editor/snap-shot-program-editor.component';
import { DocumentComponent } from './Components/document/document.component';
import { ProgramBuilderWeekViewComponent } from './Components/program-builder-week-view/program-builder-week-view.component';

const routes: Routes =
  [{ path: '', redirectTo: '/Login', pathMatch: 'full' },
  { path: 'Home', component: HomeComponent, canActivate: [AuthGuard] },
  { path: 'Login', component: LoginComponent },
  { path: 'Register', component: NewRegComponent },
  { path: 'SetRep', component: SetsAndRepsComponent, canActivate: [AuthGuard] },
  { path: 'Survey', component: SurveyComponent, canActivate: [AuthGuard] },
  { path: 'Roster', component: RosterComponent, canActivate: [AuthGuard] },
  { path: 'Multimedia', component: MultimediaComponent, canActivate: [AuthGuard] },
  { path: 'Exercise', component: ExerciseComponent, canActivate: [AuthGuard], data: { animation: 'HomePage' } },
  { path: 'Metrics', component: MetricComponent, canActivate: [AuthGuard], data: { animation: 'AboutPage' } },
  { path: 'Program', component: ProgramComponent, canActivate: [AuthGuard] },
  {
    path: 'AthleteProfile/:id', component: AthleteHomeComponent, canActivate: [AuthGuard],
    children: [
      { path: 'Workout/:athleteId', component: AthleteWorkoutComponent, canActivate: [AuthGuard] },
      { path: 'Metric/:athleteId', component: AthleteMetricComponent, canActivate: [AuthGuard] },
      { path: 'Survey/:athleteId', component: AthleteSurveyComponent, canActivate: [AuthGuard] },
      { path: 'Bio/:athleteId', component: AthleteBioComponent, canActivate: [AuthGuard] },
      { path: 'EditProgram/:athleteId', component: SnapShotProgramEditorComponent, canActivate: [AuthGuard]},
      { path: 'PastWorkouts/:athleteId', component: AthletepastWorkoutComponent, canActivate: [AuthGuard] }

    ]
  },
  {
    path: 'AthleteHome', component: AthleteHomeComponent, canActivate: [AuthGuard],
    children: [
      { path: 'Workout', component: AthleteWorkoutComponent, canActivate: [AuthGuard] },
      { path: 'Metric', component: AthleteMetricComponent, canActivate: [AuthGuard] },
      { path: 'Survey', component: AthleteSurveyComponent, canActivate: [AuthGuard] },
      { path: 'Bio', component: AthleteBioComponent, canActivate: [AuthGuard] },
      { path: 'PastWorkouts', component: AthletepastWorkoutComponent, canActivate: [AuthGuard] }
    ]
  },
  { path: 'AthleteEmailVerification/:emailToken/:athleteId', component: AthleteEmailVerificationComponent },
  { path: 'AssistantCoachEmailRegistration/:emailToken/:coachId', component: AssistantCoachEmailVerificationComponent },
  { path: 'AccountSettings', component: AccountSettingsComponent, canActivate: [AuthGuard] },
  { path: 'ProgramBuilder', component: ProgramBuilderComponent, canActivate: [AuthGuard] },
  { path: 'CoachRoster', component: CoachesRosterComponent, canActivate: [AuthGuard] },
  { path: 'OneTimeRegister/:emailToken/:coachId', component: OneTimeComponent },
  { path: 'WhiteBoardView/:programId/:theme/:week/:day', component: WhiteboardComponent, canActivate: [AuthGuard] },
  { path: 'ResetPassword/:emailToken/:userId', component: ResetPasswordComponent },
  { path: 'CreateAthlete', component: CreateAthleteComponent, canActivate: [AuthGuard] },
  { path: 'WeightRoom', component: WeightRoomComponent, canActivate: [AuthGuard] },
  { path: 'StripeData', component: StripeDataComponent },
  { path: 'Chat', component: ChatComponent, canActivate: [AuthGuard] },
  { path: 'Notifications', component: NotificationsComponent, canActivate: [AuthGuard] },
  { path: 'ProgramBuilder/:id', component: ProgramBuilderComponent, canActivate: [AuthGuard] },
  { path: 'Document', component: DocumentComponent, canActivate: [AuthGuard] },
  { path: 'ProgramBuilderWeekView', component: ProgramBuilderWeekViewComponent, canActivate: [AuthGuard] }];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
