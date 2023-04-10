import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { fadeInAnimation } from 'src/app/animation/fadeIn';
import { Movie } from 'src/app/Models/Movie';
import { Tag, TagType } from 'src/app/Models/Tag';
import { TagModel } from 'src/app/Models/TagModel';
import { TagService } from 'src/app/Services/tag-service.service';

@Component({
  selector: 'app-multimedia-form',
  templateUrl: './multimedia-form.component.html',
  styleUrls: ['./multimedia-form.component.less'],
  animations: [fadeInAnimation]
})
export class MultimediaFormComponent implements OnInit {
  @Input() CreateMovieMenu: boolean = false;
  @Input() SelectedMovie: Movie = new Movie();
  @Output() CancelCallBack = new EventEmitter<boolean>();
  @Output() SaveCallBack = new EventEmitter<any>();
  @Output() UpdateCallBack = new EventEmitter<any>();
  @Input() LabelVisible: boolean = true;

  public AllTags: TagModel[] = [];
  public newMovieTagItems: TagModel[] = [];

  constructor(public tagService: TagService) { }

  ngOnInit(): void {
  }

  AddNewEditMovieTags(s: TagModel) {
    if (this.AllTags.find(d => { return d.display == s.display }) == null) {
      var tagToAdd = new Tag();
      tagToAdd.Name = s.display;
      tagToAdd.Type = TagType.Movie;
      this.tagService.CreateTag(tagToAdd).subscribe((data) => {
        var newTM = new TagModel();
        newTM.display = s.display;
        newTM.value = data;
        this.AllTags.push(newTM)
      });
    }
    this.newMovieTagItems.push(s);
  }

  RemoveNewEditMovieTags(s: TagModel) {
    var index = this.newMovieTagItems.findIndex(x => { return x.display == s.display });
    this.newMovieTagItems.splice(index, 1);
  }

  Cancel() {
    this.CancelCallBack.emit(true)
  }

  Save(targetMovie: Movie, tags: TagModel[]) {
    var data = {targetMovie, tags};
    this.SaveCallBack.emit(data);
  }

  Update(targetMovie: Movie, tags: TagModel[]) {
    var data = {targetMovie, tags};
    this.UpdateCallBack.emit(data);
  }
}
