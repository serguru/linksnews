import { Component, Input } from '@angular/core';

@Component({
    
    selector: 'under-construction-placeholder',
    templateUrl: './under-construction.component.html'
})

export class UnderConstructionComponent {
    @Input() view: string;
}
