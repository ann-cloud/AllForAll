export class ProductRequestDto {
  constructor(
    public productName: string, 
    public country: string, 
    public manufacturer: number, 
    public category: number
  ) {}
}
