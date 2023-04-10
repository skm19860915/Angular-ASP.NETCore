import { Component, OnInit, Input, DoCheck } from '@angular/core';
import { interval} from 'rxjs';
import { take } from 'rxjs/operators';
import { fadeInAnimation } from 'src/app/animation/fadeIn';
import { AlertMessage } from '../../../Models/AlertMessage';

@Component({
  selector: 'app-alert',
  templateUrl: './alert.component.html',
  styleUrls: ['./alert.component.less'],
  animations: [fadeInAnimation]
})
export class AlertComponent implements OnInit, DoCheck {

  @Input() messages: AlertMessage[] = undefined;

  constructor() {}

  ngOnInit() {}

  ngDoCheck() {
    if(this.messages.length > 0) this.removeMessagesEvery(3);
  }

  /**
   * Pops the first message in messages array every given second(s)
   * @param seconds  Seconds to wait between pops, default is 3
   */
  removeMessagesEvery(seconds: number = 3){
    // shift() is used as array.pop() removes the last
    // item, not the first.
    interval(seconds * 1000)
    .pipe(take(1))
    .subscribe(msg => this.messages.shift());
  }

}
