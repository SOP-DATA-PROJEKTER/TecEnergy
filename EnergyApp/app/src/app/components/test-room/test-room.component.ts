import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TestService } from 'src/app/services/test.service';
import { Observable } from 'rxjs';
import { TestRoomRoomComponent } from '../test-room-room/test-room-room.component';
import {Location} from '@angular/common';
import { SimpleInfoTest } from 'src/app/models/SimpleInfoTest';
import { InfoType } from 'src/app/Enums/InfoType';

@Component({
  selector: 'app-test-room',
  standalone: true,
  imports: [CommonModule,TestRoomRoomComponent],
  templateUrl: './test-room.component.html',
  styleUrls: ['./test-room.component.css']
})
export class TestRoomComponent {
  constructor(public service : TestService, private _location: Location) {}
  Info! : Observable<SimpleInfoTest[]>;
  
  click()
  {
    this.Info = this.service.getSiblingInfo(1,2);
    this.Info.subscribe(x => console.log(x));
  }

  backClicked() 
  {
    this._location.back();
  }

  getInfoType(value : number) : string
  {
    return InfoType[value];
  }
}
