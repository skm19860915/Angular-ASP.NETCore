import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { Tag, TagType } from 'src/app/Models/Tag';
import { Observable, of, interval } from 'rxjs';
import { TagModel } from '../../Models/TagModel';
import { TagService } from '../../Services/tag-service.service';
import { Movie } from 'src/app/Models/Movie';
import { fadeInAnimation } from '../../animation/fadeIn';
import { MultiMediaService } from '../../Services/multi-media.service';
import { AlertMessage } from 'src/app/Models/AlertMessage';
import { take } from 'rxjs/operators';
import { ScpTagInputComponent } from '../shared/scp-tag-input/scp-tag-input.component';
import { ExcludeTagFilterPipe, TagFilterPipe, SearchTaggableFilterPipe } from '../../Pipes';
import { NgxSmartModalService } from 'ngx-smart-modal';
import { DisplayPaginatedItems } from '../shared/paginator/paginator.component';

@Component({
  selector: 'app-multimedia',
  templateUrl: './multimedia.component.html',
  styleUrls: ['./multimedia.component.less'],
  animations: [fadeInAnimation]
})
export class MultimediaComponent implements OnInit {
  @ViewChild(ScpTagInputComponent) scpTagInputChild: ScpTagInputComponent;
  public ShowArchive: boolean = false;
  public CreateMovieMenu: boolean = false;
  public SelectedMovie: Movie = new Movie();
  public AllTags: TagModel[] = [];
  public newMovieTagItems: TagModel[] = [];
  public AlertMessages: AlertMessage[] = [];
  public UnModifedMovies: Movie[];
  public AllMovies: Movie[];
  public searchString: string = '';
  public TagItems: TagModel[] = [];
  public AllMovieTags: TagModel[] = [];
  public targetVideoToDisplay: string = '';
  public HardDeleteTargetid : number;
  public ShowHardDeleteWindow : boolean = false;
  public paginationStart: number = 0;
  public paginationEnd : number = 0;
  constructor(private cd: ChangeDetectorRef,public modalController: NgxSmartModalService, public tagService: TagService,
    public multimediaService: MultiMediaService, public TagFilterPipe: TagFilterPipe) {
  }

  ngOnInit() {
    this.GetAllMovies();
    this.tagService.GetAllTags(TagType.Movie).subscribe(d => {
      for (var i = 0; i < d.length; i++) {
        var newTM = new TagModel();
        newTM.display = d[i].Name;
        newTM.value = d[i].Id;
        this.AllMovieTags.push(newTM)
      }
    });
  }
  UpdatePaginiationDisplay(s:DisplayPaginatedItems) {
    this.paginationStart = s.Start;
   this.paginationEnd = s.End;
   this.cd.detectChanges();
 }
  ModifySelectedMovie(targetMovie: Movie) {
    this.SelectedMovie = targetMovie;
    this.CreateMovieMenu = true;
    window.scrollTo(100, 100);
  }
  GetAllMovies() {
    this.multimediaService.GetAllMovies().subscribe(x => {
      this.AllMovies = x;
      this.UnModifedMovies = x;
    });
  }
  ViewCreateMovieMenu() {
    this.CreateMovieMenu = true;
  }
  ReturnToTagSearch() {
    this.CreateMovieMenu = false;
  }

  Save(data:any){
    this.SaveMovie(data.targetMovie, data.tags);
  }

  SaveMovie(targetMovie: Movie, tags: TagModel[]) {
    if (targetMovie.URL === '' || targetMovie.URL === undefined) {
      return;
    }
    if (targetMovie.Name === '' || targetMovie.Name === undefined) {
      return;
    }

    tags.forEach((value, index) => {
      this.AllTags.forEach((sourceTag: TagModel) => {
        if (sourceTag.display == value.display) {
          var newTag = new Tag();
          newTag.Id = sourceTag.value;
          newTag.Name = sourceTag.display;
          newTag.Type = TagType.Exercise;
          targetMovie.Tags.push(newTag);
        }
      });
    });
    this.multimediaService.CreateMovie(this.SelectedMovie).subscribe(
      success => {
        this.SelectedMovie = new Movie();
        this.newMovieTagItems = [];
        this.scpTagInputChild.ClearTags();
        this.DisplayMessage('Movie Saved Successfully', 'Movie Saved Successfully', false);
        this.ReturnToTagSearch();
        this.GetAllMovies();
      },
      error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage('Movie Saved Unsuccessfully', errorMessage, true)
      });
  }

  Update(data:any){
    this.UpdateMovie(data.targetMovie, data.tags);
  }

  UpdateMovie(targetMovie: Movie, tags: TagModel[]) {
    if (targetMovie.URL === '' || targetMovie.URL === undefined) {
      return;
    }
    if (targetMovie.Name === '' || targetMovie.Name === undefined) {
      return;
    }

    tags.forEach((value, index) => {
      this.AllTags.forEach((sourceTag: TagModel) => {
        if (sourceTag.display == value.display) {
          var newTag = new Tag();
          newTag.Id = sourceTag.value;
          newTag.Name = sourceTag.display;
          newTag.Type = TagType.Exercise;
          targetMovie.Tags.push(newTag);
        }
      });
    });
    this.multimediaService.UpdateMovie(this.SelectedMovie).subscribe(
      success => {
        this.SelectedMovie = new Movie();
        this.newMovieTagItems = [];
        this.scpTagInputChild.ClearTags();
        this.DisplayMessage('Movie Saved Successfully', 'Movie Saved Successfully', false);
        this.ReturnToTagSearch();
        this.GetAllMovies();
      },
      error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage('Movie Saved Unsuccessfully', error, true)
      });
  }

  DisplayMessage(title: string, message: string, isError: boolean) {
    const newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage)
  }
  RemoveTag(s: TagModel) {
    //this sucks, but we can no longer use the PIPE in the html. Until we figure out how to get the NGMODEL from the Parent to the child control scpTagInput
    var index = this.TagItems.findIndex(x => { return x.display == s.display });// this.TagItems.findIndex(x => x.display == s.display);
    this.AllMovieTags.splice(index, 1);
    this.TagItems.splice(index, 1);
    this.AllMovies = this.TagFilterPipe.transform(this.UnModifedMovies, this.TagItems)
  }
  AddTag(s: TagModel) {

    if (this.AllMovieTags.find(d => { return d.display == s.display }) == null) {
      var tagToAdd = new Tag();
      tagToAdd.Name = s.display;
      tagToAdd.Type = TagType.Exercise;
      this.tagService.CreateTag(tagToAdd).subscribe((data) => {
        var newTag = new TagModel()
        newTag.display = s.display;
        newTag.value = data;
        this.AllMovieTags.push(newTag);

      });
    }
    this.TagItems.push(s);
    this.AllMovies = this.TagFilterPipe.transform(this.UnModifedMovies, this.TagItems)
  };
  DisplayTargetVideo(videoURL: string) {
    this.targetVideoToDisplay = videoURL;
    this.modalController.setModalData({ url: videoURL }, 'multimediaVideoModal');
    this.modalController.open('multimediaVideoModal')
    window.scrollTo(50, 50);
  }
  ArchiveVideo(videoId: number) {
    this.multimediaService.ArchiveMovie(videoId).subscribe(success => {
      this.DisplayMessage('Movie Archived Successfully', 'Movie Archived Successfully', false);
      this.GetAllMovies();
    }, error => {
      var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
      this.DisplayMessage('Movie Archived Unsuccessfully', error, true)
    });

  }
  UnArchiveVideo(videoId: number)
  {
    this.multimediaService.UnArchiveMovie(videoId).subscribe(success => {
      this.DisplayMessage('Movie UnArchived Successfully', 'Movie UnArchived Successfully', false);
      this.GetAllMovies();
    }, error => {
      var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
      this.DisplayMessage('Movie UnArchived Unsuccessfully', error, true)
    });

  }
  ToggleArchive() {
    this.ShowArchive = !this.ShowArchive;
  }
  public HardDelete(movieId: number) {
    this.multimediaService. HardDeleteMovie(movieId).subscribe(success =>{
      this.ToggleHardDeleteModal(0);
      this.DisplayMessage("Video DELETED", "Video Successfully Deleted", false);
      this.GetAllMovies();
    },error =>{
      var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
      this.ToggleHardDeleteModal(0);
      this.DisplayMessage("Video NOT DELETED",errorMessage,  true);
    });
  }
  public ToggleHardDeleteModal(movieId: number) {
    this.ShowHardDeleteWindow = !this.ShowHardDeleteWindow;
    this.HardDeleteTargetid = movieId;
  }
}
