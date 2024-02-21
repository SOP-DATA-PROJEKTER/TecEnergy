import {Component, EventEmitter, Inject, Input, OnInit, Output} from '@angular/core';
import {MAT_DATE_LOCALE} from '@angular/material/core';
import {MatButtonModule} from '@angular/material/button';
import {MatDatepicker, MatDatepickerInputEvent, MatDatepickerModule } from '@angular/material/datepicker';
import {MatInputModule} from '@angular/material/input';
import {MatFormFieldModule} from '@angular/material/form-field';
import { provideMomentDateAdapter } from '@angular/material-moment-adapter';
import { FormControl } from '@angular/forms';
import * as Highcharts from 'highcharts';
import { HighchartsChartModule } from 'highcharts-angular';

import { MatNativeDateModule } from '@angular/material/core';
import {  FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import 'moment/locale/da';
import * as moment from 'moment';
import { DateValue } from 'src/app/models/DateValue';
import { GraphService } from 'src/app/services/graph.service';
import { ActivatedRoute } from '@angular/router';


@Component({
    selector: 'app-meterdetailpage',
    templateUrl: './meterdetailpage.component.html',
    styleUrls: ['./meterdetailpage.component.css'],
    providers: [
        // The locale would typically be provided on the root module of your application. We do it at
        // the component level here, due to limitations of our example generation script.
        { provide: MAT_DATE_LOCALE, useValue: 'da-DK' },
        // Moment can be provided globally to your app by adding `provideMomentDateAdapter`
        // to your app config. We provide it at the component level here, due to limitations
        // of our example generation script.
        provideMomentDateAdapter(),
    ],
    standalone: true,
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
        MatButtonModule
    ]
})

export class MeterdetailpageComponent implements OnInit{

  @Input() meterId!: string;
  @Output() meterDetailEvent = new EventEmitter<boolean>();

  Highcharts: typeof Highcharts = Highcharts;
  chartOptions: any;
  date: FormControl;
  dataDaily: DateValue[] = [];
  isDaily: boolean = true;
  isMonthly: boolean = true;
  isYearly: boolean = true;

  constructor(
    @Inject(MAT_DATE_LOCALE) private _locale: string,
    private graphService: GraphService,
    private route: ActivatedRoute, 
  ) 
  {
    this.date = new FormControl(new Date());

    if(!this.meterId){
      this.meterId = this.route.snapshot.paramMap.get('id')!;
    }

  }


  
  ngOnInit(): void {

    this.newChart();


    this.fetchDailyData();
    this.fetchMonthlyData();
    this.fetchYearlyData();
    

  }

  fetchDailyData(){
    
    this.isDaily = true;

    this.graphService.getDailyGraphData(this.meterId, new Date(this.date.value)).subscribe({
      next: (data) => {
        if(data.length <= 0){
          this.isDaily = false;
          console.error('No data');
        }
        else{
          this.updateChart(data, 'daily');
          Highcharts.chart('graph-daily', this.chartOptions);
        }
      },
      error: (error) => {
        console.error(error);
      },
      complete: () => {
      }
    })

  }

  fetchMonthlyData(){

    this.isMonthly = true;

    this.graphService.getMonthlyGraphData(this.meterId, new Date(this.date.value)).subscribe({
      next: (data) => {
        if(data.length <= 0){
          console.error('No data');
          this.isMonthly = false;
        } 
        else{ 
          this.updateChart(data, 'monthly');
          Highcharts.chart('graph-monthly', this.chartOptions);
        }
      },
      error: (error) => {
        console.error(error);
      },
      complete: () => {
      }
    })

  }

  fetchYearlyData(){

    this.isYearly = true;

    this.graphService.getYearlyGraphData(this.meterId).subscribe({
      next: (data) => {
        if(data.length <= 0){
          this.isYearly = false;
          console.error('No data');
        } else{
          this.updateChart(data, 'yearly');
          Highcharts.chart('graph-yearly', this.chartOptions);
        }
      },
      error: (error) => {
        console.error(error);
      },
      complete: () => {
      }
    })

  }


  
  updateChart(dataDaily: DateValue[], type: string) {
    // updates the chart data and rerenders it

    switch(type){
      case 'daily':
        this.chartDaily(dataDaily);
        break;
      case 'monthly':
        this.chartMonhtly(dataDaily);
        break;
      case 'yearly':
        this.chartYearly(dataDaily);
        break;
    }


    // rerenders the chart
    // Highcharts.chart('graph-daily', this.chartOptions);
    
  }

  chartDaily(dataDaily: DateValue[]){

    this.chartOptions.series![0] = {
      name: 'Daily Accumulated Value',
      data: dataDaily.map(x => x.accumulatedValue),
      color: '#00FF00',
      dataLabels: {
        enabled: true,
        color: '#ffffff',
        style: {
          textOutline: '1px contrast'
        }
      }
    }


    this.chartOptions.xAxis! ={
      categories: dataDaily.map(x => new Date(x.date).toLocaleDateString('da-DK', {
        day: '2-digit',
      })),
      labels: {
        style: {
          color: 'white'
        }
      }
    }


    this.chartOptions.title = {
      text: 'Daily Accumulated Values',
      style: {
        color: 'white'
      }
    }


  }

  chartMonhtly(dataDaily: DateValue[]){

    this.chartOptions.series![0] = {
      name: 'Monthly Accumulated Value',
      data: dataDaily.map(x => x.accumulatedValue),
      color: '#00FF00',
      dataLabels: {
        enabled: true,
        color: '#ffffff',
        style: {
          textOutline: '1px contrast'
        }
      }
    }


    this.chartOptions.xAxis! ={
      categories: dataDaily.map(x => new Date(x.date).toLocaleDateString('da-DK', {
        month: 'long',
      })),
      labels: {
        style: {
          color: 'white'
        }
      }
    }

    this.chartOptions.title = {
      text: 'Monthly Accumulated Values',
      style: {
        color: 'white'
      }
    }

  }

  chartYearly(dataDaily: DateValue[]){

    this.chartOptions.series![0] = {
      name: 'Yearly Accumulated Value',
      data: dataDaily.map(x => x.accumulatedValue),
      color: '#00FF00',
      dataLabels: {
        enabled: true,
        color: '#ffffff',
        style: {
          textOutline: '1px contrast'
        }
      }
    }


    this.chartOptions.xAxis! ={
      categories: dataDaily.map(x => new Date(x.date).toLocaleDateString('da-DK', {
        year: 'numeric',
      })),
      labels: {
        style: {
          color: 'white'
        }
      }
    }

    this.chartOptions.title = {
        text: 'Yearly Accumulated Values',
        style: {
          color: 'white'
        }
      }
    
  }



  // event on datepicker change
  // take date and emit it to parent componenet


  newChart(){
        // Initializes chart
        this.chartOptions = 
        {
          chart: {
            type: 'column',
            backgroundColor: 'transparent',
            width: 500
          },
          xAxis: {
            categories: [],
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
            name: 'Monthly Accumulated Value',
            data: [],
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
  }


  onChange($event: any, datepicker: MatDatepicker<any>) {
    this.date.setValue(new Date($event._i.year, $event._i.month, $event._i.date));
    datepicker.close();
    this.fetchDailyData();
    this.fetchMonthlyData();
    }

}