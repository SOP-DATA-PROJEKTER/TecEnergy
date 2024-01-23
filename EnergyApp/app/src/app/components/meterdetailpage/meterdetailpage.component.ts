import {Component, EventEmitter, Inject, OnInit, Output} from '@angular/core';
import {MAT_DATE_LOCALE} from '@angular/material/core';
import {MatButtonModule} from '@angular/material/button';
import {MatDateRangePicker, MatDatepicker, MatDatepickerModule } from '@angular/material/datepicker';
import {MatInputModule} from '@angular/material/input';
import {MatFormFieldModule} from '@angular/material/form-field';
import { provideMomentDateAdapter } from '@angular/material-moment-adapter';
import { FormControl, FormGroup } from '@angular/forms';
import { YearlyAccumulatedDto } from 'src/app/models/YearlyAccumulatedDto';
import * as Highcharts from 'highcharts';
import { HighchartsChartModule } from 'highcharts-angular';
import { RoomService } from 'src/app/services/room.service';
import { ActivatedRoute } from '@angular/router';
import { MatNativeDateModule } from '@angular/material/core';
import {  FormsModule, ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { CommonModule } from '@angular/common';
import 'moment/locale/da';
import * as moment from 'moment';


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
    ]
})

export class MeterdetailpageComponent implements OnInit {
  @Output() meterDetailEvent = new EventEmitter<boolean>();

  Highcharts: typeof Highcharts = Highcharts;
  chartOptions: any;
  date: FormControl;
  id: string;
  data: YearlyAccumulatedDto[] = [];

  constructor(
    private roomService : RoomService, 
    private route: ActivatedRoute, 
    private formBuilder: FormBuilder,
    @Inject(MAT_DATE_LOCALE) private _locale: string,
  ) 
  {

    const momentStart = {
      month: 0,
      date: 1,
      year: 2023
    }

    this.date = new FormControl(moment(momentStart));
      
    this.id = this.route.snapshot.paramMap.get('id')!;
  }


  
  ngOnInit(): void {

    this.updateDataFromMoment()
    
    // Initializes chart
      this.chartOptions = 
      {
        chart: {
          type: 'column',
          backgroundColor: 'transparent'
        },
        title: {
          text: 'Monthly Accumulated Values',
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

  updateDataYearly(Date: String){

    this.roomService.getRoomYearlyAccumulationList(this.id, Date).subscribe((data: YearlyAccumulatedDto[]) => {
      this.data = data;
      this.updateChart();
    });
  
  }
  
  updateChart() {
    // updates the chart data and rerenders it

    this.chartOptions.series![0] = {
          name: 'Monthly Accumulated Value',
          data: this.data.map(x => x.MonthlyAccumulatedValue),
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
      categories: this.data.map(x => new Date(x.Month).toLocaleDateString('da-DK', {
        month: 'long'
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
    console.log("updateDataFromMoment ")
    // formats a moment into MM-DD-YYYY format and calls function to update data
    const val = this.date.value._i;

    const dateString = `${val.month+1}-${val.date}-${val.year}`

    this.updateDataYearly(dateString);
  }

  goBack() {
    this.meterDetailEvent.emit(true);
  }

  setDate($event: any, datepicker: MatDatepicker<any>) {
    this.date.setValue($event);
    datepicker.close();
    this.updateDataFromMoment();    
  }

}
