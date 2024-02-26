import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MeterData } from 'src/app/models/MeterData';

@Component({
  selector: 'app-meterlist-item',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './meterlist-item.component.html',
  styleUrls: ['./meterlist-item.component.css']
})
export class MeterlistItemComponent 
{
  Ticks : number[] = [];
  @Input() fontSize : number = 110;

  @Input() Data! : MeterData;

  constructor() 
  {
    for ( var i = 0; i < 20; i++)
    {
      this.Ticks.push(i);
    }
  }
}
