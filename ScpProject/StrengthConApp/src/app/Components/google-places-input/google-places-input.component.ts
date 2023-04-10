//@ts-ignore
/// <reference types="@types/googlemaps" />

import { Component, ViewChild, EventEmitter, Output, OnInit, AfterViewInit, Input } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-google-places-input',
  templateUrl: './google-places-input.component.html',
  styleUrls: ['./google-places-input.component.css']
})
export class GooglePlacesInputComponent implements OnInit, AfterViewInit {
  @Input() addressType: string;
  @Output() setAddress: EventEmitter<any> = new EventEmitter();
  @ViewChild('addresstext') addresstext: any;

  autocompleteInput: string;
  queryWait: boolean;

  constructor() { }

  ngOnInit(): void {
  }


  ngAfterViewInit() {
    this.getPlaceAutocomplete();
  }

  private getPlaceAutocomplete() {
    //@ts-ignore
    const autocomplete = new google.maps.places.Autocomplete(this.addresstext.nativeElement,
      {
      });
      //@ts-ignore
    google.maps.event.addListener(autocomplete, 'place_changed', () => {
      const place = autocomplete.getPlace();
      this.invokeEvent(place);
    });
  }

  invokeEvent(place: Object) {
    this.setAddress.emit(place);
  }
}

