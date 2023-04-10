import { ColorPickerModule } from 'ngx-color-picker';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './Components/Login/Login.component';
import { RegisterComponent } from './Components/Register/Register.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { CookieService } from 'ngx-cookie-service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ExerciseComponent } from './Components/Exercise/Exercise.component';
import { NavMenuComponent } from './Components/nav-menu/nav-menu.component';
import { RouterModule } from '@angular/router';
import { UserRibbonComponent } from './Components/user-ribbon/user-ribbon.component';
import { SearchableDropDownComponent } from './Components/shared/searchable-drop-down/searchable-drop-down.component';
import { SearchThroughTagsPipe, BasicStringSearchPipe, DropDownListItemFilterPipe, TagFilterPipe, SearchTaggableFilterPipe, ExcludeTagFilterPipe, ArraySortPipe, HideDeletedSortPipe, EncodeUriPipe, SafePipe } from './Pipes';
import { SetsAndRepsComponent } from './Components/sets-and-reps/sets-and-reps.component';
import { NgxSmartModalModule } from 'ngx-smart-modal';
import { SurveyComponent } from './Components/survey/survey.component';
import { MultimediaComponent } from './Components/multimedia/multimedia.component';
import { TagInputModule } from 'ngx-chips';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RosterComponent } from './Components/roster/roster.component';
import { DragScrollModule } from 'ngx-drag-scroll';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { MetricComponent } from './Components/metric/metric.component';
import { ProgramBuilderComponent } from './Components/program-builder/program-builder.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { ProgramComponent } from './Components/program/program.component';
import { FileUploadModule } from 'ng2-file-upload';
import { HomeComponent } from './Components/home/home.component';
import { ImagePreview } from './directives/image-preview.directive';
import { AthleteHomeComponent } from './Components/athlete-home/athlete-home.component';
import { AthleteLiftingComponent } from './athlete-lifting/athlete-lifting.component';
import { AthleteWorkoutComponent } from './Components/athlete-workout/athlete-workout.component';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { ResetPasswordComponent } from './Components/reset-password/reset-password.component';
import { AthleteEmailVerificationComponent } from './Components/athlete-email-verification/athlete-email-verification.component';
import { AccountSettingsComponent } from './Components/account-settings/account-settings.component';
import { UserEmailVerificationComponent } from './Components/user-email-verification/user-email-verification.component';
import { CoachesRosterComponent } from './Components/coaches-roster/coaches-roster.component';
import { AssistantCoachEmailVerificationComponent } from './Components/assistant-coach-email-verification/assistant-coach-email-verification.component';
import { OneTimeComponent } from './Components/one-time/one-time.component';
import { DpDatePickerModule } from 'ng2-date-picker'
import { WhiteboardComponent } from './Components/whiteboard/whiteboard.component';
import { ScpTagInputComponent } from './Components/shared/scp-tag-input/scp-tag-input.component';
import { CreateAthleteComponent } from './Components/create-athlete/create-athlete.component';
import { AthleteMetricComponent } from './Components/athlete-metric/athlete-metric.component';

import { WeightRoomComponent } from './Components/weight-room-components/weight-room/weight-room.component';
import { StripeDataComponent } from './Components/stripe-data/stripe-data.component';
import { ThrivecartTestComponent } from './Components/thrivecart-test/thrivecart-test.component';
import { CustomerValidatorInterceptor } from './Interceptors/CustomerValidatorInterceptor';
import { ChatComponent } from './Components/chat/chat.component';
import { NotificationsComponent } from './Components/notifications/notifications.component';
import { AngularEditorModule } from '@kolkov/angular-editor';
import { AthleteSurveyComponent } from './Components/athlete-survey/athlete-survey.component';
import { AthleteBioComponent } from './Components/athlete-bio/athlete-bio.component';
import { AthletepastWorkoutComponent } from './Components/athletepast-workout/athletepast-workout.component';
import { Step1Component } from './Components/newRegistration/step1/step1.component';
import { Step2Component } from './Components/newRegistration/step2/step2.component';
import { Step3Component } from './Components/newRegistration/step3/step3.component';
//import { UpdateStripeCardComponent } from './Components/update-stripe-card/update-stripe-card.component';
import { NewRegComponent } from './Components/newRegistration/new-reg/new-reg.component';
import { Step4Component } from './Components/newRegistration/step4/step4.component';
import { UnreadNotificationsComponent } from './Components/dashboard/unread-notifications/unread-notifications.component';
import { UnreadMesagesComponent } from './Components/dashboard/unread-mesages/unread-mesages.component';
import { ProgramStatusComponent } from './Components/dashboard/program-status/program-status.component';
import { AtheltesWithoutProgramComponent } from './Components/dashboard/atheltes-without-program/atheltes-without-program.component';
import { GooglePlacesInputComponent } from './Components/google-places-input/google-places-input.component';
import { TwoDropDownSearchComponent } from './Components/shared/two-drop-down-search/two-drop-down-search.component'
import { CreditCardDirectivesModule } from 'angular-cc-library';
import { PaginatorComponent } from './Components/shared/paginator/paginator.component';
import { AlertComponent } from "./Components/shared/alert/alert.component";
import { WeightRoomAthleteHeaderComponent } from './Components/weight-room-components/weight-room-athlete-header/weight-room-athlete-header.component';
import { WeightRoomSingleViewComponent } from './Components/weight-room-components/weight-room-single-view/weight-room-single-view.component';
import { WeightRoomSplitViewComponent } from './Components/weight-room-components/weight-room-split-view/weight-room-split-view.component';
import { WeightRoomTrifoldViewComponent } from './Components/weight-room-components/weight-room-trifold-view/weight-room-trifold-view.component';
import { WeightRoomGridViewComponent } from './Components/weight-room-components/weight-room-grid-view/weight-room-grid-view.component';
import { NgxDaterangepickerMd } from 'ngx-daterangepicker-material';
import { SnapShotProgramEditorComponent } from './Components/snap-shot-program-editor/snap-shot-program-editor.component';
import { SetsAndRepsFormComponent } from "./Components/shared/sets-and-reps-form/sets-and-reps-form.component";
import { MetricFormComponent } from './Components/shared/metric-form/metric-form.component';
import { SurveyFormComponent } from './Components/shared/survey-form/survey-form.component';
import { MultimediaFormComponent } from './Components/shared/multimedia-form/multimedia-form.component';
import { SetsAndRepsTableComponent } from './Components/shared/sets-and-reps-table/sets-and-reps-table.component';
import { DocumentComponent } from './Components/document/document.component';
import { DocumentFormComponent } from './Components/shared/document-form/document-form.component';
import { MetricFormMeasurementComponent } from './Components/shared/metric-form-measurement/metric-form-measurement.component';
import { ProgramBuilderWeekViewComponent } from './Components/program-builder-week-view/program-builder-week-view.component';
@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    ExerciseComponent,
    NavMenuComponent,
    UserRibbonComponent,
    SearchableDropDownComponent,
    DropDownListItemFilterPipe,
    SearchThroughTagsPipe,
    SetsAndRepsComponent,
    SurveyComponent,
    MultimediaComponent,
    TagFilterPipe,
    RosterComponent,
    MetricComponent,
    SearchTaggableFilterPipe,
    ProgramBuilderComponent,
    ProgramComponent,
    ImagePreview,
    HomeComponent,
    ExcludeTagFilterPipe,
    AthleteHomeComponent,
    AthleteLiftingComponent,
    AthleteWorkoutComponent,
    ResetPasswordComponent,
    AthleteEmailVerificationComponent,
    AccountSettingsComponent,
    ArraySortPipe,
    HideDeletedSortPipe,
    UserEmailVerificationComponent,
    CoachesRosterComponent,
    AssistantCoachEmailVerificationComponent,
    OneTimeComponent,
    WhiteboardComponent,
    EncodeUriPipe,
    SafePipe,
    ScpTagInputComponent,
    CreateAthleteComponent,
    BasicStringSearchPipe,
    AthleteMetricComponent,
    WeightRoomComponent,
    StripeDataComponent,
    ThrivecartTestComponent,
    ChatComponent,
    NotificationsComponent,
    AthleteSurveyComponent,
    AthleteBioComponent,
    AthletepastWorkoutComponent,
    Step1Component,
    NewRegComponent,
    Step2Component,
    Step3Component,
    Step4Component,
    UnreadNotificationsComponent,
    UnreadMesagesComponent,
    ProgramStatusComponent,
    AtheltesWithoutProgramComponent,
    GooglePlacesInputComponent,
    TwoDropDownSearchComponent,
    PaginatorComponent,
    AlertComponent,
    WeightRoomAthleteHeaderComponent,
    WeightRoomSingleViewComponent,
    WeightRoomSplitViewComponent,
    WeightRoomTrifoldViewComponent,
    WeightRoomGridViewComponent,
    SnapShotProgramEditorComponent,
    SetsAndRepsFormComponent,
    MetricFormComponent,
    SurveyFormComponent,
    MultimediaFormComponent,
    SetsAndRepsTableComponent,
    DocumentComponent,
    DocumentFormComponent,
    MetricFormMeasurementComponent,
    ProgramBuilderWeekViewComponent,
    // UpdateStripeCardComponent,
    //PdfPrinterComponent
  ],
  imports: [
    DragScrollModule,
    DpDatePickerModule,
    TagInputModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    RouterModule,
    NgxSmartModalModule.forRoot(),
    DragDropModule,
    NgSelectModule,
    FileUploadModule,
    NgxChartsModule,
    AngularEditorModule,
    ColorPickerModule,
    CreditCardDirectivesModule,
    NgxDaterangepickerMd.forRoot(),
  ],
  providers:
    [CookieService,
      ExcludeTagFilterPipe,
      SearchTaggableFilterPipe,
      TagFilterPipe,
      ArraySortPipe,
      HideDeletedSortPipe,
      SearchThroughTagsPipe,
      {
        provide: HTTP_INTERCEPTORS,
        useClass: CustomerValidatorInterceptor,
        multi: true
      }],

  bootstrap: [AppComponent]
})
export class AppModule {

}
