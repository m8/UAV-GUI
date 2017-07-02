#include<Wire.h>
#include  <SPI.h> // SPI Arayüzünü dahil ettik.
#include "nRF24L01.h" // RF24 kütüpanelerini dahil ettik.
#include "RF24.h" // RF24 kütüpanelerini dahil ettik.
int msg[1]; //Göndereceğimiz mesaj değişkenini oluşturduk.
char msgs[1];
RF24 radio(9,10); //RF24 kütüphanesi tarafından kullanılacak olan 2 pinin 9. ve 10. pinlerin olduğunu belirttik.
const uint64_t pipe = 0xE8E8F0F0E1LL; // Pipe (kanal) tanımlamasını yaptık.
String theMessage = "";
const int MPU = 0x68;
/* MPU-6050'nin I2C haberleşme adresi */

int16_t AcX, AcY, AcZ, Tmp, GyX, GyY, GyZ;
/* IMU'dan alınacak değerlerin kaydedileceği değişkenler */

void setup() {
  Wire.begin();
  Wire.beginTransmission(MPU);
  Wire.write(0x6B);
  Wire.write(0); /* MPU-6050 çalıştırıldı */
  Wire.endTransmission(true);
  radio.begin(); // Kablosuz iletişimi başlattık.
  radio.openWritingPipe(pipe); //Gönderim yapacağımız kanalın ID'sini tanımladık.
  Serial.begin(9600);
}
void loop() {
  verileriOku();
  AcY = map(AcY,-17000,17000,-45,45);
  AcX = map(AcX,-17000,17000,-45,45);
    //Serial.print("ivmeX = "); Serial.print(AcX);
   // Serial.println(AcY+1);
  // String theMessage = "hello there!";
  String theMessage = String(AcY+1);
  int messageSize = theMessage.length();
  Serial.println(theMessage); 
  for (int i = -1; i < messageSize; i++) {
    char charToSend[1];
    charToSend[0] = theMessage.charAt(i);
    radio.write(charToSend,1);
  }  
  
 msg[0] = '%';
 radio.write(msg, 1);//Sonra da bu mesaj gönderilsin.
 theMessage = "";
 delay(1);
 
}

void verileriOku() {
  Wire.beginTransmission(MPU);
  Wire.write(0x3B);
  Wire.endTransmission(false);
  Wire.requestFrom(MPU, 14, true);
  AcX = Wire.read() << 8 | Wire.read();
  AcY = Wire.read() << 8 | Wire.read();
  AcZ = Wire.read() << 8 | Wire.read();
  Tmp = Wire.read() << 8 | Wire.read();
  GyX = Wire.read() << 8 | Wire.read();
  GyY = Wire.read() << 8 | Wire.read();
  GyZ = Wire.read() << 8 | Wire.read();

}
