import {Component, EventEmitter, Inject, OnInit, Output} from '@angular/core';
import {MAT_DATE_LOCALE} from '@angular/material/core';
import {MatButtonModule} from '@angular/material/button';
import {MatDateRangePicker, MatDatepicker, MatDatepickerModule } from '@angular/material/datepicker';
import {MatInputModule} from '@angular/material/input';
import {MatFormFieldModule} from '@angular/material/form-field';
import { provideMomentDateAdapter } from '@angular/material-moment-adapter';
import { FormGroup } from '@angular/forms';
import { DailyAccumulatedDto } from 'src/app/models/DailyAccumulatedDto';
import * as Highcharts from 'highcharts';
import { HighchartsChartModule } from 'highcharts-angular';
import { RoomService } from 'src/app/services/room.service';
import { ActivatedRoute } from '@angular/router';
import { MatNativeDateModule } from '@angular/material/core';
import {  FormsModule, ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { CommonModule } from '@angular/common';
import 'moment/locale/da';
import * as moment from 'moment';
import { CarouselComponent } from "../carousel/carousel.component";


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
        MatButtonModule,
        CarouselComponent
    ]
})

export class MeterdetailpageComponent implements OnInit {


  @Output() meterDetailEvent = new EventEmitter<boolean>();


  Highcharts: typeof Highcharts = Highcharts;
  chartOptions: any;
  

  range: FormGroup;
  id: string;

  data: DailyAccumulatedDto[] = [];


  constructor(
    private roomService : RoomService, 
    private route: ActivatedRoute, 
    private formBuilder: FormBuilder,
    @Inject(MAT_DATE_LOCALE) private _locale: string,
  ) 
  {

    const startMoment = {
      month: 11,
      date: 12,
      year: 2023
    }
    
    const newDate = new Date();

    const endMoment = {
      month: newDate.getMonth(),
      date: newDate.getDate(),
      year: newDate.getFullYear()
    }


    this.range = this.formBuilder.group({
        start: moment(startMoment),
        end: moment(endMoment)
      });
  
      this.id = this.route.snapshot.paramMap.get('id')!;
      
    }


  
  ngOnInit(): void {

    this.updateDataFromMoment();
    
    // Initializes chart
      this.chartOptions = 
      {
        chart: {
          type: 'column',
          backgroundColor: 'transparent'
        },
        title: {
          text: 'Daily Accumulated Values',
          style: {
            color: 'white'
          }
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
          name: 'Daily Accumulated Value',
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


  updateData(startDate: String, endDate: String){

    // fetches new data based on input range

    this.roomService.getRoomDailyAccumulationList(this.id, startDate, endDate).subscribe((data: DailyAccumulatedDto[]) => {
      this.data = data;
      this.updateChart();
    });
  }
  
  updateChart() {

    // updates the chart data and rerenders it

    this.chartOptions.series![0] = {
          name: 'Daily Accumulated Value',
          data: this.data.map(x => x.DailyAccumulatedValue),
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
      categories: this.data.map(x => new Date(x.DateTime).toLocaleDateString('da-DK', {
        year: 'numeric',
        month: 'numeric',
        day: 'numeric'
      })),
      labels: {
        style: {
          color: 'white'
        }
      }
    }



    // rerenders the chart
    Highcharts.chart('container', this.chartOptions);
    
  }


  updateDataFromMoment(){
    // formats a moment into MM-DD-YYYY format and calls function to update data

    const startValue = this.range.value.start._i;
    const endValue = this.range.value.end._i;

    const start = `${startValue.month+1}-${startValue.date}-${startValue.year}`
    const end =  `${endValue.month+1}-${endValue.date}-${endValue.year}`

    this.updateData(start, end);
  }

  goBack() {
    this.meterDetailEvent.emit(true);
  }

}
