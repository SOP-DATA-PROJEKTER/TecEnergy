import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../navbar/navbar.component';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { RoomService } from 'src/app/services/room.service';
import { SimpleInfo } from 'src/app/models/SimpleInfo';
import { MeterData } from 'src/app/models/MeterData';
import { DashboardComponent } from '../dashboard/dashboard.component';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-room',
  standalone: true,
  imports: [CommonModule,NavbarComponent,SidebarComponent,DashboardComponent],
  templateUrl: './room.component.html',
  styleUrls: ['./room.component.css']
})
export class RoomComponent implements OnInit
{
  constructor(private roomService : RoomService, private route : ActivatedRoute, private router: Router) {}

  update : boolean = true;

  CurrentRoomId : string = "0";
  Room : MeterData = {Id : "0", Name : "", RealTime : 0, Accumulated : 0, Note : ""}
  Meters : MeterData[] = [];

  Building : SimpleInfo = {Id : "0", Name : ""}
  RoomList : SimpleInfo[] = [];

  ngOnInit(): void 
  {
    this.route.params.subscribe(params => 
    {
      this.CurrentRoomId = params['id'];
      // console.log(this.CurrentRoomId)
      this.roomService.getMeterData(params['id']).subscribe(x => this.Room = x);
      this.roomService.getSubMeterData(params['id']).subscribe(x => this.Meters = x);
    });

    this.roomService.getParentSimpleInfo().subscribe(x => this.Building = x);
    this.roomService.getSiblingsSimpleInfo().subscribe(x => this.RoomList = x);


    setTimeout(() => {
        this.UpdateMeters();
     }, 5000);
  }

  UpdateMeters() : void
  {
    this.roomService.getMeterData(this.CurrentRoomId).subscribe(x => this.Room = x);
    this.roomService.getSubMeterData(this.CurrentRoomId).subscribe(x => this.Meters = x);
    // console.log(this.Meters)

    setTimeout(() => {
      if(this.update)
      {
        this.UpdateMeters();
      }
     }, 5000);
  }

  ngOnDestroy() 
  {
    this.update = false;
  }

  SideBarClick(id:string)
  {
    this.router.navigate(['room', id]);
  }
}
