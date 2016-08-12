module app.common {
    export interface ISerializable<T> {
        deserialize(input: Object): T;
    }
}