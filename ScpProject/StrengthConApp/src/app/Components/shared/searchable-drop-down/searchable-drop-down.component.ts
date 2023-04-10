import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { DropDownListItem } from "../../../Models/DropDownListItem"


@Component({
  selector: 'app-searchable-drop-down',
  templateUrl: './searchable-drop-down.component.html',
  styleUrls: ['./searchable-drop-down.component.less']
})



export class SearchableDropDownComponent implements OnInit {
  @Input() list: DropDownListItem[];
  @Input() placeholderText: string;
  @Output() selectedItemChange = new EventEmitter<DropDownListItem>();
  public selectedItem: DropDownListItem = new DropDownListItem();
  public searchText: string;
  public hideDataHolder: boolean;
  constructor() {
    this.hideDataHolder = true;

  }

  ngOnInit() {
  }

  ChangeSelectedItem(item: DropDownListItem) {
    this.selectedItemChange.emit(item);
    this.selectedItem = item;
    this.searchText = item.Name;
  }

  focus(): void { this.hideDataHolder = false }
  focusOut(): void { this.hideDataHolder = true; }

}

