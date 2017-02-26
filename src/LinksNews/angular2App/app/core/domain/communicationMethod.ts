import * as _ from 'lodash';

export class CommunicationMethod {
    id: number;
    name: string;

    clone(): CommunicationMethod {
        let result: CommunicationMethod = <CommunicationMethod>_.assign(new CommunicationMethod(), this);
        return result;
    }

    static from (account: CommunicationMethod): CommunicationMethod {
        let result: CommunicationMethod = new CommunicationMethod();
        _.assign(result, account);
        return result;
    }
}
