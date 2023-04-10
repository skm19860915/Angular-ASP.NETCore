import { Component, OnInit } from '@angular/core';
import { ProgramBuilderService } from '../../Services/program-builder.service';
import { Program } from '../../Models/Program/Program';
import { NgxSmartModalService } from 'ngx-smart-modal';
import { ProgramDayItem } from '../../Models/Program/ProgramDayItem';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-whiteboard',
  templateUrl: './whiteboard.component.html',
  styleUrls: ['./whiteboard.component.less']
})
export class WhiteboardComponent implements OnInit {
  public AllPrograms: Program[] = [];
  public View: string = "All Programs";
  public SelectedProgram: Program = new Program();
  public Theme: number = 0; //0 == dark, 1 == light
  public TotalDays: number = 0;
  public TargetDay: number = 0;
  public TargetWeek: number = 1;
  public ItemPosition = 0;
  public p: any = new ProgramDayItem();
  public FontSize: number = .5;
  public Height: number = 1000;
  public targetVideoToDisplay: string = '';
  constructor(private route: ActivatedRoute, public programBuilderService: ProgramBuilderService, public ngxSmartModalService: NgxSmartModalService) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.SelectProgram(params['programId']);
      this.TargetDay = params["day"]
      this.TargetWeek = parseInt(params["week"]);
      this.SetTheme(params["theme"]);
    });
  }
  IncreaseFont() {
    this.FontSize += .125;
    this.Height += 50;

  }
  DecreaseFontSize() {
    this.FontSize -= .125;
    this.Height -= 50;
  }
  ShowWeek(weekIds: number[], currentWeek) {
    return weekIds.find(x => x == currentWeek) >= 0;
  }
  SetTheme(theme) {

    this.Theme = theme;
    let darkCSS = "background: #4e4e4e url(/assets/appBgImg.png) no-repeat center center !important;  background-size      : cover !important    background-attachment: fixed !important;    overflow             : hidden !important;";
    switch (theme) {
      case "0"://light
        // @ts-ignore

        break;
      case "1"://dark
        // @ts-ignore
        document.getElementById('mainContent').style = darkCSS;
        // @ts-ignore
        document.getElementsByTagName('html').style = "background-color: #4e4e4e";
        break;
      default:
        // @ts-ignore
        document.getElementsByTagName('html').style = "background-color: #4e4e4e";
        break;
    }


  }

  AdvanceItem() {
    if (this.ItemPosition == this.SelectedProgram.Days[this.TargetDay].Items.length - 1) {
      this.ItemPosition = 0;
    }
    else {
      this.ItemPosition++;
    }
    this.p = this.SelectedProgram.Days[this.TargetDay].Items.filter(item => item.Position == this.ItemPosition)[0];
  }
  DecreaseItem() {
    if (this.ItemPosition == 0) {
      this.ItemPosition = this.SelectedProgram.Days[this.TargetDay].Items.length - 1;
    }
    else {
      this.ItemPosition--;
    }
    this.p = this.SelectedProgram.Days[this.TargetDay].Items.filter(item => item.Position == this.ItemPosition)[0];
  }
  Start() {
    this.p = this.SelectedProgram.Days[this.TargetDay].Items.filter(item => item.Position == this.ItemPosition)[0];
  }

  SelectProgram(programId: number) {
    this.programBuilderService.GetProgram(programId).subscribe(program => {
      this.SelectedProgram = program;
      this.SelectedProgram.Days.forEach(x => {
        x.Items = x.Items.filter(y => y.ItemType != 2)//we dont want surveys to display
        x.Items.sort((a,b) => a.Id - b.Id).forEach((item,index) => item.Position = index );//since we potentionall can be removing an element we need to re-order
        //to be sequentiol with out gaps
      })
      this.TotalDays = program.Days.length;
      this.Start();

    });
  }

  DisplayTargetVideo(videoURL: string) {
    this.targetVideoToDisplay = videoURL;
    this.ngxSmartModalService.setModalData({ url: videoURL }, 'exerciseVideoModal');
    this.ngxSmartModalService.open('exerciseVideoModal')
  }

  DisplayProgram(day: number, weekNumber: number, invertColors: boolean) {

  }
}
