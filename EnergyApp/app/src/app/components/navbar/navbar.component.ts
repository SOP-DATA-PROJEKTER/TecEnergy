import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {

  @Input() Title : string = "Test Title";

  fontSize : number = 20;

  Test(element: HTMLElement, pw : number)
  {
    var x = this.fontSize/element.offsetWidth;
    var final = pw * x;

    console.log("Width : " + element.offsetWidth);
    console.log("Height : " + this.fontSize);
    console.log("X : " + x);
    console.log("final : " + final);
    this.fontSize = final;
  }
}
