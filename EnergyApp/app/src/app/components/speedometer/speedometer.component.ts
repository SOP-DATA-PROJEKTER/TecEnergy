import { Component, ElementRef, HostListener, Input, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MeterData } from 'src/app/models/MeterData';
import { TooltipDirective } from 'src/app/directives/tooltip.directive';


interface Tick
{
  rotation : number;
  index : number;
  value : number;
}

@Component({
  selector: 'app-speedometer',
  standalone: true,
  imports: [CommonModule, TooltipDirective],
  templateUrl: './speedometer.component.html',
  styleUrls: ['./speedometer.component.css']
})
export class SpeedometerComponent
{
  @Input() TopValue = 60;

  Ticks : Tick[] = [];

  fontSize : number = 110;
  baseColor : string = "black";
  activeColor : string = "#0f0";
  maxColor : string = "#ff2a00";

  lineHeight : number = 6;
  tickWidth : number = 1;
  halfCircleHeight : number = 250;

  @ViewChild("Display") display! : ElementRef;
  @ViewChild("Circle") circle! : ElementRef;


  @Input() Data! : MeterData;


  
  constructor() 
  {
    var tickInterval = this.TopValue / 60;

    for ( var i = 0; i < 61; i++)
    {
      this.Ticks.push({ rotation:4 * i - 120, index : i , value: i * tickInterval});
    }
  }

  ngAfterViewInit()
  {
    this.onResize();
  }

  @HostListener('window:resize')
  onResize() {
    var circleHeight = this.circle.nativeElement.offsetHeight;
    var displayWidth = this.display.nativeElement.offsetWidth;

    this.fontSize = Math.round(displayWidth * 0.39);
    this.tickWidth = Math.round(circleHeight/100);
    this.halfCircleHeight = Math.round(circleHeight/2);
    this.lineHeight = Math.round(displayWidth/50);
  }
  
  getFontSize(i:number)
  {
    return {'font-size': this.fontSize/i + 'px'};
  }

  getLineHeight()
  {
    return {'height': this.lineHeight + 'px'};
  }
    
  SetTick(tick: Tick) 
  {
    var baseStyles = {
      'position': 'absolute',
      'background-color': this.baseColor,
      'left': '50%',
      'top': '0',
      'width': `${this.tickWidth}px`,
      'height': `${tick.index % 10 === 0 ? 15 : 10}%`,
      'transform-origin': `0 ${this.halfCircleHeight}px`,
      'transform': `rotate(${tick.rotation}deg)`
    };

    if(this.Data.realTime >= this.TopValue)
    {
      baseStyles['background-color'] = this.maxColor;
      (baseStyles as any)['box-shadow'] = `0 0 25px ${this.maxColor}, 0 0 50px ${this.maxColor}`;
    }

    else if(tick.value <= this.Data.realTime)
    {
      baseStyles['background-color'] = this.activeColor;
      (baseStyles as any)['box-shadow'] = `0 0 25px ${this.activeColor}, 0 0 50px ${this.activeColor}`;
    }

    return baseStyles;
  }
}
