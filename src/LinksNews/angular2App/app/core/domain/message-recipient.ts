import * as _ from 'lodash';

export class MessageRecipient {
    id: number;
    messageId: number;
    recipientAccountId: number;
    recipientAddress: string;
    receiveDate: Date;
    communicationMethodId: number;

    clone(): MessageRecipient {
        let result: MessageRecipient = <MessageRecipient>_.assign(new MessageRecipient(), this);
        return result;
    }

    static from (account: MessageRecipient): MessageRecipient {
        let result: MessageRecipient = new MessageRecipient();
        _.assign(result, account);
        return result;
    }
}
