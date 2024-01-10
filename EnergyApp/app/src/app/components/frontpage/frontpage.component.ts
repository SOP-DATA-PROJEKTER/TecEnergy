import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../navbar/navbar.component';
import { RoomService } from 'src/app/services/room.service';
import { SimpleRoom } from 'src/app/models/SimpleRoom';

@Component({
  selector: 'app-frontpage',
  standalone: true,
  imports: [CommonModule,NavbarComponent],
  templateUrl: './frontpage.component.html',
  styleUrls: ['./frontpage.component.css']
})
export class FrontpageComponent implements OnInit
{
  room: SimpleRoom = {
    Id: ""
  };

  constructor(private roomService : RoomService) 
  {}
  
  ngOnInit(): void {
    this.roomService.getInitialRoomId().subscribe(x => this.room = x);
  }

}
