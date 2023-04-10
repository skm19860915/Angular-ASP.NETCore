// import { Component, OnInit } from '@angular/core';
// import { ProgramBuilderService } from '../../Services/program-builder.service';
// import * as jspdf from 'jspdf';
// import html2canvas from 'html2canvas';
// import { Program } from '../../Models/Program/Program';
// import { ActivatedRoute } from '@angular/router';
// import { html2PDF } from 'jspdf-html2canvas';
// import pdfMake from 'pdfmake/build/pdfmake';
// import pdfFonts from 'pdfmake/build/vfs_fonts';
// pdfMake.vfs = pdfFonts.pdfMake.vfs;

// @Component({
//   selector: 'app-pdf-printer',
//   templateUrl: './pdf-printer.component.html',
//   styleUrls: ['./pdf-printer.component.less']
// })
// export class PdfPrinterComponent implements OnInit {
//   public Program: Program;
//   public ProgramService: ProgramBuilderService;

//   constructor(ProgramBuilderService: ProgramBuilderService, private route: ActivatedRoute) {
//     this.ProgramService = ProgramBuilderService;
//     this.Program = this.GenerateEmptyProgram();
//   }
//   ngOnInit() {
//     this.route.params.subscribe(params => {
//       this.ProgramService.GetProgram(params['id']).subscribe(x => {
//         this.Program = x;
//         console.log(x)
//       });
//     });
//   }
//   fuck() {
//     var data = document.getElementById('WorkoutBody');
//     html2canvas(data).then(canvas => {
//       // Few necessary setting options  
//       var imgWidth = 208;
//       var pageHeight = 295;
//       var imgHeight = canvas.height * imgWidth / canvas.width;
//       var heightLeft = imgHeight;

//       const contentDataURL = canvas.toDataURL('image/png')
//       let pdf = new jspdf('p', 'mm', 'a4'); // A4 size page of PDF  
//       var position = 0;
//       pdf.addImage(contentDataURL, 'PNG', 0, position, imgWidth, imgHeight)
//       pdf.save('MYPdf.pdf'); // Generated PDF 
//       //   var doc = new jsPDF({unit: 'px', format: 'letter'});
//       //       // source can be HTML-formatted string, or a reference
//       //       // to an actual DOM element from which the text will be scraped.
//       //   var source = window.document.getElementById("WorkoutBody");
//       //   // doc.fromHTML(
//       //   //   source,
//       //   //   15,
//       //   //   15,
//       //   //   {
//       //   //     'width': 800
//       //   //   }, function (dispose) { doc.save('Workout.pdf') });
//       //  // window["html2canvas"] = html2canvas;
//       //  html2PDF(source, {
//       //   jsPDF: {},
//       //   imageType: 'image/jpeg',
//       //   output: './pdf/generate.pdf'
//       // });
//       // //   doc.html(
//       // //     source,
//       // //     function (dispose) { doc.save('Workout.pdf') });
//     });
//   }
//   GenerateEmptyProgram(): Program {
//     var ret = new Program();
//     ret.WeekCount = 1
//     ret.Days = [{
//       Id: 1,
//       IsActive: true,

//       Items: [],
//       Position: 1
//     }];
//     return ret;
//   }
// }

// // var doc = new jsPDF();
// // var elementHandler = {
// //   '#ignorePDF': function (element, renderer) {
// //     return true;
// //   }
// // };
// // var source = window.document.getElementById("steve");
// // doc.fromHTML(
// //   source,
// //   15,
// //   15,
// //   {
// //     'width': 800, 'elementHandlers': elementHandler
// //   }, function (dispose) { doc.save('test.pdf') });

// // // doc.output("dataurlnewwindow");