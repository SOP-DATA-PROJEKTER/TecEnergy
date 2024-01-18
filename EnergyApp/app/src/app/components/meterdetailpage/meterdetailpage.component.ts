import { Component, OnInit} from '@angular/core';
import * as Highcharts from 'highcharts';
import { HighchartsChartModule } from 'highcharts-angular';
import { RoomService } from 'src/app/services/room.service';
import { DailyAccumulatedDto } from 'src/app/models/DailyAccumulatedDto';
import { ActivatedRoute } from '@angular/router';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatNativeDateModule } from '@angular/material/core';
import { FormGroup, FormsModule, ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { CommonModule } from '@angular/common';



@Component({
  selector: 'app-meterdetailpage',
  standalone: true,
  imports: [
    CommonModule,
    HighchartsChartModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatInputModule,
    MatNativeDateModule,
    FormsModule,
    ReactiveFormsModule

  ],
  templateUrl: './meterdetailpage.component.html',
  styleUrls: ['./meterdetailpage.component.css']
})
export class MeterdetailpageComponent implements OnInit {

  Highcharts: typeof Highcharts = Highcharts;
  chartOptions: any;
  

  range: FormGroup;
  id: string;

  data: DailyAccumulatedDto[] = [];

  constructor(private roomService : RoomService, private route: ActivatedRoute, private formBuilder: FormBuilder) {
    this.range = this.formBuilder.group({
      start: new Date(2023, 11, 12),
      end: new Date()
    });

    this.id = this.route.snapshot.paramMap.get('id')!;
    
  }



  ngOnInit(): void {
    this.updateData()
    
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
          categories: this.data,
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
          data: this.data.map(x => x.DailyAccumulatedValue),
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


  updateData(){



    this.roomService.getRoomDailyAccumulationList(this.id, this.range.value.start, this.range.value.end).subscribe((data: DailyAccumulatedDto[]) => {
      this.data = data;
      this.updateChart();
    });
    
    
  }
  
  updateChart() {

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
      categories: this.data.map(x => new Date(x.DateTime).toLocaleDateString('dk-DK', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit'
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
  

}