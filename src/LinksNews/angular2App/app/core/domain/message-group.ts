import * as _ from 'lodash';

export class MessageGroup {
    id: number;
    name: string;

    clone(): MessageGroup {
        let result: MessageGroup = <MessageGroup>_.assign(new MessageGroup(), this);
        return result;
    }

    static from (account: MessageGroup): MessageGroup {
        let result: MessageGroup = new MessageGroup();
        _.assign(result, account);
        return result;
    }
}
