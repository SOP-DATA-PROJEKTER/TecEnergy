import { DOCUMENT } from '@angular/common';
import { ComponentRef, Directive, ElementRef, HostListener, Inject, Input } from '@angular/core';
import {ViewContainerRef} from '@angular/core';
import { TooltipComponent } from '../components/tooltip/tooltip.component';

@Directive({
  selector: '[tooltip]',
  standalone: true
})
export class TooltipDirective {

  //The text to write in the tooltip if it is left empty it will not activate
  @Input() tooltipText = "";

  //Ref for the component made by this directive
  private tooltipComponent?: ComponentRef<any>;
  
  //Creates and appends a tooltip component to the body
  @HostListener('mouseenter')
  onMouseEnter():void
  {
    //Check to see if a tooltip should be made
    if(this.tooltipComponent || this.tooltipText == "")
    {
      return;      
    }

    //Creates and appends the tooltip
    this.tooltipComponent = this.viewContainerRef.createComponent(TooltipComponent);
    this.document.body.appendChild(this.tooltipComponent.location.nativeElement);
    

    //Sets the position of the tooltip relative to the element it its on
    const {left,top,width, height} = this.elementRef.nativeElement.getBoundingClientRect();
    this.tooltipComponent.instance.left = left + width / 2;
    this.tooltipComponent.instance.top = top + height / 2;
    this.tooltipComponent.instance.text = this.tooltipText;
  }


  //Removes the created tooltip
  @HostListener('click')
  @HostListener('mouseleave')
  onMouseLeave():void
  {
    //Check that there is a tooltip
    if(!this.tooltipComponent)
    {
      return;      
    }

    //Removes the tooltip 
    this.viewContainerRef.clear();
    this.tooltipComponent = undefined;
  }


  constructor(
    private viewContainerRef: ViewContainerRef,
    private elementRef: ElementRef,
    @Inject(DOCUMENT) private document: Document,
    ) { }

}
