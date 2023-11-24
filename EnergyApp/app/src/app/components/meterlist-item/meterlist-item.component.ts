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

  @Input() Data : MeterData = {Name: "Lokale xxx", Current : 0, Accumulated: 999999, Note: "Test Note"};

  constructor() 
  {
    for ( var i = 0; i < 20; i++)
    {
      this.Ticks.push(i);
    }
  }
  
  HasNote() : boolean
  {
    if(this.Data.Note != undefined && this.Data.Note != "")
      return true;

    return false;
  }
}
