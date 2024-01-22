import { Component, Input, OnInit,  Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SpeedometerComponent } from '../speedometer/speedometer.component';
import { MeterData } from 'src/app/models/MeterData';
import { MeterlistItemComponent } from '../meterlist-item/meterlist-item.component';
import { ActivatedRoute, Router } from '@angular/router';
import { TooltipDirective } from 'src/app/directives/tooltip.directive';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule,SpeedometerComponent,MeterlistItemComponent,TooltipDirective],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit
{
  @Input() MainMeter! : MeterData;
  @Input() SubMeters! : MeterData[];

  @Output() detailEvent = new EventEmitter<boolean>();
  
  CurrentRoomId : string = "0";

  constructor(private router: Router, private route : ActivatedRoute) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => 
    {
      this.CurrentRoomId = params['id'];
    });
  }

  ShowListView() : boolean
  {
    if(this.SubMeters.length > 4)
      return true;

    return false;
  }
  


  TestGoToDetails()
  {
    // emit event
    this.detailEvent.emit(false);
    // this.router.navigate([`meterdetail/${this.CurrentRoomId}`]);
  }
}
