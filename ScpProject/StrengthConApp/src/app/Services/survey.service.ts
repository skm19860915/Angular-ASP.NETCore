import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { Survey } from '../Models/Survey';
import { Question, QuestionType, MappedQuestionToSurvey } from '../Models/Question';
import { CompletedScaleQuestion } from '../Models/CompletedSurvey/CompletedScaleQuestion';
import { CompletedYesNoQuestion } from '../Models/CompletedSurvey/CompletedYesNoQuestion';
import { CompletedOpenEndedQuestion } from '../Models/CompletedSurvey/CompletedOpenEndedQuestion';
import { HistoricProgram } from '../Models/Program/HistoricProgram';
import { AthleteHomePageSurvey } from '../Models/Survey/AthleteHomePageSurvey';
import { AssignedQuestion } from '../Models/Program/AssignedProgram';
import { PastSurveyListItem } from '../Models/Survey/PastSurveyListItem';



@Injectable({
  providedIn: 'root'
})
export class SurveyService {
  private _headers;
  constructor(private http: HttpClient, private cookieService: CookieService) {
    this._headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin': '*',
      'Access-Control-Allow-Credentials': 'true'
    });

  }
  GetAllPastSurveys(athleteId: number): Observable<PastSurveyListItem[]> {
    return this.http.get<PastSurveyListItem[]>(environment.endpointURL + "/api/Survey/GetPastSurveyList/" + athleteId
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  }
  GetAllSurveysForProgram(assignedProgramId): Observable<AthleteHomePageSurvey[]> {
    return this.http.get<AthleteHomePageSurvey[]>(environment.endpointURL + "/api/Survey/GetSurveysByAssignedProgramId/" + assignedProgramId
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  }

  GetAllSurveyQuestions(surveyId: number): Observable<MappedQuestionToSurvey[]> {
    return this.http.get<MappedQuestionToSurvey[]>(environment.endpointURL + "/api/Survey/GetAllSurveyQuestions/" + surveyId
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  };

  GetAllQuestions(): Observable<Question[]> {
    return this.http.get<Question[]>(environment.endpointURL + "/api/Survey/GetAllQuestions/"
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  };

  CreateSurvey(newSurvey: Survey): Observable<number> {
    return this.http.post<number>(environment.endpointURL + "/api/Survey/Create/"
      , JSON.stringify(newSurvey)
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  };

  AddQuestionToSurvey(surveyId: number, questionId: number) {
    return this.http.post<number>(environment.endpointURL + "/api/Survey/AddQuestionToSurvey/"
      , JSON.stringify({ QuestionId: questionId, SurveyId: surveyId })
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  };
  CreateQuestion(newQuestion: Question): Observable<number> {
    return this.http.post<number>(environment.endpointURL + "/api/Survey/CreateQuestion/"
      , JSON.stringify(newQuestion)
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  };

  UpdateSurvey(newSurvey: Survey) {
    return this.http.post(environment.endpointURL + "/api/Survey/UpdateSurvey/"
      , JSON.stringify(newSurvey)
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  };

  RemoveQuestionToSurvey(SurveysToQuestionsId: number) {
    return this.http.post<number>(environment.endpointURL + "/api/Survey/RemoveQuestionFromSurvey/" + SurveysToQuestionsId
      , JSON.stringify({})
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  };

  UpdateQuestion(newQuestion: Question) {
    return this.http.post(environment.endpointURL + "/api/Survey/UpdateQuestion/"
      , JSON.stringify(newQuestion)
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  };
  GetQuestionDetails(questionId: number) : Observable<Question>{
    return this.http.get<Question>(environment.endpointURL + `/api/Survey/GetQuestionDetails/${questionId}`, {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  GetAllSurveys(): Observable<Survey[]> {
    return this.http.get<Survey[]>(environment.endpointURL + "/api/Survey/GetAllSurveys/", {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }

  GetAllHistoricSurveys(athleteId: number): Observable<HistoricProgram[]> {
    return this.http.get<HistoricProgram[]>(environment.endpointURL + "/api/Survey/GetHistoricProgramsWithSurveys/" + athleteId, {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }

  GetAssignedProgramSurveyAnswers(athleteId: number, programId: number): Observable<AssignedQuestion[]> {
    return this.http.get<AssignedQuestion[]>(environment.endpointURL + "/api/Survey/GetAssignedProgramQuestions/" + athleteId + "/" + programId, {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }

  AnswerYesNoQuestion(yesNoValue, questionId, programDayItemSurveyId, weekId, athleteId: number) {
    var completedQuestion = new CompletedYesNoQuestion();
    completedQuestion.YesNoValue = yesNoValue;
    completedQuestion.QuestionId = questionId;
    completedQuestion.ProgramDayItemSurveyId = programDayItemSurveyId;
    completedQuestion.WeekId = weekId;
    completedQuestion.AthleteId = athleteId;
    return this.http.post(environment.endpointURL + "/api/Survey/AnswerYesNoQuestion"
      , JSON.stringify(completedQuestion)
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  }
  AnswerScaleQuestion(scaleValue, questionId, programDayItemSurveyId, weekId, athleteId: number) {
    var completedQuestion = new CompletedScaleQuestion();
    completedQuestion.ScaleValue = scaleValue;
    completedQuestion.QuestionId = questionId;
    completedQuestion.ProgramDayItemSurveyId = programDayItemSurveyId;
    completedQuestion.WeekId = weekId;
    completedQuestion.AthleteId = athleteId;
    return this.http.post(environment.endpointURL + "/api/Survey/AnswerScaleQuestion"
      , JSON.stringify(completedQuestion)
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  }
  AnswerOpenEndedQuestion(response, questionId, programDayItemSurveyId, weekId, athleteId: number) {
    var completedQuestion = new CompletedOpenEndedQuestion();
    completedQuestion.Response = response;
    completedQuestion.QuestionId = questionId;
    completedQuestion.ProgramDayItemSurveyId = programDayItemSurveyId;
    completedQuestion.WeekId = weekId;
    completedQuestion.AthleteId = athleteId;
    return this.http.post(environment.endpointURL + "/api/Survey/AnswerOpenEndedQuestion"
      , JSON.stringify(completedQuestion)
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  }

  DuplicateSurvey(surveyId: number): Observable<number> {
    return this.http.post<number>(environment.endpointURL + `/api/Survey/DuplicateSurvey/${surveyId}`
      , '', {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }
  HardDeleteSurvey(surveyId: number): any {
    return this.http.get(environment.endpointURL + `/api/Survey/HardDelete/${surveyId}`
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      })
  }
  ArchiveSurvey(surveyId: number): any {
    return this.http.post<number>(environment.endpointURL + `/api/Survey/ArchiveSurvey/${surveyId}`
      , '', {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }
  UnArchiveSurvey(surveyId: number): any {
    return this.http.post<number>(environment.endpointURL + `/api/Survey/UnArchiveSurvey/${surveyId}`
      , '', {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }
}
