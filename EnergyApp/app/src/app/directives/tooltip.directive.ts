import { DOCUMENT } from '@angular/common';
import { ComponentRef, Directive, ElementRef, HostListener, Inject, Input } from '@angular/core';
import {ViewContainerRef} from '@angular/core';
import { TooltipComponent } from '../components/tooltip/tooltip.component';

@Directive({
  selector: '[tooltip]',
  standalone: true
})
export class TooltipDirective {

  @Input() tooltipText = "";

  private tooltipComponent?: ComponentRef<any>;
  
  @HostListener('mouseenter')
  onMouseEnter():void
  {
    if(this.tooltipComponent || this.tooltipText == "")
    {
      return;      
    }
    this.tooltipComponent = this.viewContainerRef.createComponent(TooltipComponent);
    this.document.body.appendChild(this.tooltipComponent.location.nativeElement);
    
    const {left,top,width, height} = this.elementRef.nativeElement.getBoundingClientRect();
    this.tooltipComponent.instance.left = left + width / 2;
    this.tooltipComponent.instance.top = top + height / 2;
    this.tooltipComponent.instance.text = this.tooltipText;
  }

  @HostListener('mouseleave')
  onMouseLeave():void
  {
    if(!this.tooltipComponent)
    {
      return;      
    }

    this.viewContainerRef.clear();
    this.tooltipComponent = undefined;
  }

  constructor(
    private viewContainerRef: ViewContainerRef,
    private elementRef: ElementRef,
    @Inject(DOCUMENT) private document: Document,
    ) { }

}
