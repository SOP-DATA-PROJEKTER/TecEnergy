import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SimpleInfo } from 'src/app/models/SimpleInfo';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-test-room-room',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './test-room-room.component.html',
  styleUrls: ['./test-room-room.component.css']
})
export class TestRoomRoomComponent {
  @Input() Data! : Observable<SimpleInfo>;
}
