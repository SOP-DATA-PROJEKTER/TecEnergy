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
  RoomDataStream$! : Observable<RoomData>;

  //New
  CurrentRoom : SimpleInfo = {id :"0", name:"" };
  RoomList : SimpleInfo[] = [];
  MainMeter : MeterData = {id :"0",name : "",realTime : 0, accumulated : 0, isConnected : false}
  SubMeters : MeterData[] = [];
  

  ngOnInit(): void 
  {
    this.roomService.getAllRoomsSimpleInfo().subscribe(x => 
    {
      this.RoomList = x;
      this.route.params.subscribe(params => 
        {
          //Set Current Room and handle if id == 0
          if(params['id'] == 0)
          {
            this.CurrentRoom.id = this.RoomList[0].id;
            this.CurrentRoom.name = this.RoomList[0].name;
          }
          else
          {
            this.CurrentRoom.id = params['id'];
            this.CurrentRoom.name = this.RoomList.filter(x => x.id = params['id'])[0].name;
          }

          //HandleRoomDataStream
  
          this.RoomDataStream$ = this.roomService.getRoomDataStream(this.CurrentRoom.id, 5000);
          this.RoomDataStream$.subscribe(x => {this.MainMeter = x.mainMeter; this.SubMeters = x.subMeters; console.log(x)});
      });

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
