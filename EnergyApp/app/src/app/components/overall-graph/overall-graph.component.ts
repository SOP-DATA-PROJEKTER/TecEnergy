import {Component, OnInit} from '@angular/core';
import {MAT_DATE_LOCALE} from '@angular/material/core';
import {MatButtonModule} from '@angular/material/button';
import {MatDatepickerModule } from '@angular/material/datepicker';
import {MatInputModule} from '@angular/material/input';
import {MatFormFieldModule} from '@angular/material/form-field';
import { provideMomentDateAdapter } from '@angular/material-moment-adapter';
import * as Highcharts from 'highcharts';
import { HighchartsChartModule } from 'highcharts-angular';
import { RoomService } from 'src/app/services/room.service';
import { ActivatedRoute } from '@angular/router';
import { MatNativeDateModule } from '@angular/material/core';
import {  FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AllAccumulatedData } from 'src/app/models/AllAccumulatedData';

@Component({
  selector: 'app-overall-graph',
  templateUrl: './overall-graph.component.html',
  styleUrls: ['./overall-graph.component.css'],
  standalone: true,
  providers: [
    // The locale would typically be provided on the root module of your application. We do it at
    // the component level here, due to limitations of our example generation script.
    { provide: MAT_DATE_LOCALE, useValue: 'da-DK' },
    // Moment can be provided globally to your app by adding `provideMomentDateAdapter`
    // to your app config. We provide it at the component level here, due to limitations
    // of our example generation script.
    provideMomentDateAdapter(),
],
imports: [
    CommonModule,
    HighchartsChartModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatInputModule,
    MatNativeDateModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
]
})
export class OverallGraphComponent implements OnInit{

  Highcharts: typeof Highcharts = Highcharts;
  chartOptions: any;
  id: string;
  data: AllAccumulatedData[] = [];

  constructor(
    private roomService : RoomService, 
    private route: ActivatedRoute,
  ) 
  {
    
    this.id = this.route.snapshot.paramMap.get('id')!;
  }


  ngOnInit(): void {

    this.updateDataYearly();
    
  }


  initialzeChart(){

    this.chartOptions = 
    {
      chart: {
        type: 'column',
        backgroundColor: 'transparent'
      },
      title: {
        text: 'Yearly Accumulated Values',
        style: {
          color: 'white'
        }
      },
      xAxis: {
        categories: this.data.map(x => new Date(x.Date).toLocaleDateString('da-DK',
        {
          year: 'numeric',
        })
        ),
        labels: {
          style: {
            color: 'white'
          }
        }
      },
      yAxis: {
        title: {
          text: 'Accumulated Value',
          style: {
            color: 'white'
          }
        },
        labels: {
          style: {
            color: 'white'
          }
        }
      },
      series: [{
        name: 'Yearly Accumulated Value',
        data: this.data.map(x => x.Accumulated),
        color: '#00FF00',
        dataLabels: {
          enabled: true,
          color: '#ffffff',
          style: {
            textOutline: '1px contrast'
          }
        }
      }],
      legend: {
        itemStyle: {
            color: 'white'
        }
      }
    };

    Highcharts.chart('chart', this.chartOptions);

  }


  updateDataYearly(){

    this.roomService.getRoomAllAccumulationList(this.id).subscribe({
      next: (data) =>{
        this.data = data;
        this.initialzeChart();
      },
      error: (err) => console.log(err),
      complete: () => console.log("complete")

    })
  }
  
  
}