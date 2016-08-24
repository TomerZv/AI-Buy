module app.businessObjects {
    export enum AuctionStatus {
        Future,
        Open,
        Close
    }

    export interface IAuction extends app.common.ISerializable<IAuction> {
        Id : number;
        ItemType : string;
        StartDate : Date;
        Duration : number;
        EndDate : Date;
        MinimumPrice : number;
        AvgPrice : number;
        MinBid : number;
        Biddings: app.businessObjects.IBid[];
        getStatus(): AuctionStatus;
        getCurrentBid(): app.businessObjects.IBid;
        getCurrentPrice(): number;
        isMultiAgentWinning(): boolean;
    }

    export class Auction implements IAuction {
        public Id: number; 
        public ItemType: string;
        public StartDate: Date;
        public Duration: number;
        public EndDate: Date;
        public MinimumPrice: number;
        public AvgPrice: number;
        public MinBid: number;
        public Biddings: app.businessObjects.IBid[];
        
        getStatus(): AuctionStatus {
            // TEMP - change this.
            return AuctionStatus.Close;
        }

        getCurrentBid(): app.businessObjects.IBid {
            var bid;
            if (this.Biddings.length == 0) {
                bid = null;
            }
            else {
                bid = this.Biddings[this.Biddings.length - 1];
            }

            return bid;
        }

        getCurrentPrice(): number {
            return this.getCurrentBid().Price;
        }

        isMultiAgentWinning(): boolean {
            var bid = this.getCurrentBid();

            return ((bid != null) && (bid.Username.indexOf("Multi") != -1));
        }

        deserialize(input): IAuction {
            this.Id = input.Id;
            this.ItemType = input.ItemType;
            this.StartDate = new Date(input.StartDate);
            this.Duration = input.Duration;
            this.EndDate = new Date(input.EndDate);
            this.MinimumPrice = input.MinimumPrice;
            this.AvgPrice = input.AvgPrice;
            this.MinBid = input.MinBid;
            this.Biddings = new Array(0);
            input.Biddings.forEach(bid => this.Biddings.push(new Bid().deserialize(bid)));

            return this;
        }
    }
} 