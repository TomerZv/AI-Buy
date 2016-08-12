module app.businessObjects {
    export interface IBid extends app.common.ISerializable<IBid> {
        Id: string;
        AuctionId: number;
        Price: number;
        Username: string;
        Date: Date;
    }

    export class Bid implements IBid {
        public Id: string;
        public AuctionId: number;
        public Price: number;
        public Username: string;
        public Date: Date;

        deserialize(input): IBid{
            this.Id = input.Id;
            this.AuctionId = input.AuctionId;
            this.Price = input.Price;
            this.Username = input.Username;
            this.Date = new Date(input.Date);

            return this;
        }
    }
} 