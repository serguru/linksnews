import { Injectable } from '@angular/core';
import { Subject }    from 'rxjs/Subject';

@Injectable()
export class AppCommunicateService {

    // Observable string sources
    public came = new Subject<any>();
    public gone = new Subject<any>();
    public contentHeightChanged = new Subject<number>();
    public wait = new Subject<boolean>();

    // Observable string streams
    came$ = this.came.asObservable();
    gone$ = this.gone.asObservable();
    contentHeightChanged$ = this.contentHeightChanged.asObservable();
    wait$ = this.wait.asObservable();
    

    // Service message commands
    processCame(data: any) {
        this.came.next(data);
    }

    processGone(data: any) {
        this.gone.next(data);
    }

    processContentHeightChanged(height: number) {
        this.contentHeightChanged.next(height);
    }

    processWait(value: boolean) {
        this.wait.next(value);
    }
}

