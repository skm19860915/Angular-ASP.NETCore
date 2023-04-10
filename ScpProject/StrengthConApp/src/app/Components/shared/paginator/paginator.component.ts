import { Component, OnInit, Input, Output, EventEmitter, AfterViewChecked } from '@angular/core';

@Component({
  selector: 'app-paginator',
  templateUrl: './paginator.component.html',
  styleUrls: ['./paginator.component.css']
})
export class PaginatorComponent implements OnInit {

  @Input() set TotalItemCount(value: number) {
    this._totalItemCount = value;
    this.UpdateTotalItems();
  };

  @Output() ItemDisplayChange = new EventEmitter<DisplayPaginatedItems>();
  private _totalItemCount: number;
  private currentPage: number = 1;
  private displayAmount: number = 5;
  private totalPageCount: number;
  public displayPages: number[] = [];
  private pagesToDisplay: number = 10;
  constructor() { }

  ngOnInit(): void {
    this.UpdateTotalItems();
  }
  UpdateTotalItems() {
    this.totalPageCount = Math.ceil(this._totalItemCount / this.displayAmount);
    this.displayPages = [];
    let startingPageCount = this.totalPageCount < this.pagesToDisplay ? this.totalPageCount : this.pagesToDisplay;
    for (var i = 1; i <= startingPageCount; i++) {
      this.displayPages.push(i);
    }
    this.UpdateDisplay();
  }
  NextClick() {

    if (this.currentPage === this.totalPageCount) {
      return;
    }
    if (this.currentPage % this.pagesToDisplay === 0) {
      this.displayPages = this.displayPages.map(x => x = x + this.pagesToDisplay);
    }
    if (this.currentPage !== this.totalPageCount) {
      this.currentPage++;
    }

    this.UpdateDisplay();
  }
  PreviousClick() {
    if (this.currentPage === 1) {
      return;
    }
    if (this.currentPage !== 1) {
      this.currentPage--;
    }
    if (this.currentPage === 1) {
      return;
    }
    if (this.currentPage % this.pagesToDisplay === 0) {
      this.displayPages = this.displayPages.map(x => x = x - this.pagesToDisplay);
    }



    this.UpdateDisplay();
  }
  PageClick(targetPage: number) {
    this.currentPage = targetPage;
    this.UpdateDisplay();
  }
  UpdateDisplay() {
    var itemsRange: DisplayPaginatedItems = new DisplayPaginatedItems();
    //since currentPage isnt 0 index we need to -1 from it
    itemsRange.Start = (this.currentPage - 1) * this.displayAmount;
    itemsRange.End = ((this.currentPage) * this.displayAmount) - 1
    this.ItemDisplayChange.emit(itemsRange)
  }
}

export class DisplayPaginatedItems {
  public Start: number;
  public End: number;
}
