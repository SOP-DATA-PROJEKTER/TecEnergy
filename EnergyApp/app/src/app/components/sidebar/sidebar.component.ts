import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SimpleInfo } from 'src/app/models/SimpleInfo';
import { Observable } from 'rxjs';
@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent 
{
  @Input() List : SimpleInfo[] = [];
  @Input() ActiveId : string = "1";
  @Output() OnClicked = new EventEmitter<string>();

  Click(id : string)
  {
    this.OnClicked.emit(id);
  }
}
