import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../navbar/navbar.component';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { RoomService } from 'src/app/services/room.service';
import { SimpleInfo } from 'src/app/models/SimpleInfo';
import { MeterData } from 'src/app/models/MeterData';
import { DashboardComponent } from '../dashboard/dashboard.component';
import { ActivatedRoute, Router } from '@angular/router';
import { MeterdetailpageComponent } from "../meterdetailpage/meterdetailpage.component";
import { Observable } from 'rxjs';
import { RoomData } from 'src/app/models/RoomData';

@Component({
    selector: 'app-room',
    standalone: true,
    templateUrl: './room.component.html',
    styleUrls: ['./room.component.css'],
    imports: [CommonModule, NavbarComponent, SidebarComponent, DashboardComponent, MeterdetailpageComponent],

})
export class RoomComponent implements OnInit
{
  constructor(private roomService : RoomService, private route : ActivatedRoute, private router: Router) {}

  showMainContent : boolean = false;

  CurrentRoomId : string = "0";

  // CurrentRoom! : SimpleInfo;

  RoomData$! : Observable<RoomData>;
  RoomList$! : Observable<SimpleInfo[]>;

  ngOnInit(): void 
  {
    this.route.params.subscribe(params => 
      {
        this.CurrentRoomId = params['id'];
        this.RoomList$ = this.roomService.getAllRoomsSimpleInfo();
        this.RoomData$ = this.roomService.getRoomDataStream(params['id'], 5000);
    });
  }

  SideBarClick(id:string)
  {
    this.router.navigate(['room', id]);
    this.showMainContent = true;
  }

  onEmitEvent(event : boolean){
    this.showMainContent = event;
  }
}
