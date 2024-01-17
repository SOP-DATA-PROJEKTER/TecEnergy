import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import * as Highcharts from 'highcharts';
import { HighchartsChartModule } from 'highcharts-angular';
import { RoomService } from 'src/app/services/room.service';
import { DailyAccumulatedDto } from 'src/app/models/DailyAccumulatedDto';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-meterdetailpage',
  standalone: true,
  imports: [
    CommonModule,
    HighchartsChartModule
  ],
  templateUrl: './meterdetailpage.component.html',
  styleUrls: ['./meterdetailpage.component.css']
})
export class MeterdetailpageComponent implements OnInit {
  Highcharts: typeof Highcharts = Highcharts;
  chartOptions: any;

  constructor(private roomService : RoomService,  private route : ActivatedRoute) {}


  ngOnInit(): void {
    this.route.params.subscribe(params => {
      const startTime = new Date(2023, 12 , 11, 0, 0, 0);
      const endTime = new Date(2024, 1 , 17, 0, 0, 0 );

      

      this.roomService.getRoomDailyAccumulationList(params['id'], startTime, endTime).subscribe((data: DailyAccumulatedDto[]) => {
        const dates = data.map(x => x.DateTime);
        const values = data.map(x => x.DailyAccumulatedValue);
  
        this.chartOptions = {
          chart: {
            type: 'column'
          },
          title: {
            text: 'Daily Accumulated Values'
          },
          xAxis: {
            categories: dates
          },
          yAxis: {
            title: {
              text: 'Accumulated Value'
            }
          },
          series: [{
            name: 'Daily Accumulated Value',
            data: values
          }]
        };

        Highcharts.chart('container', this.chartOptions);
  
      })

    });

    
    
  }




}
