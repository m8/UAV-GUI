#include <SPI.h>
#include "nRF24L01.h"
#include "RF24.h"
char msg[1];
RF24 radio(9,10);
const uint64_t pipe = 0xE8E8F0F0E1LL;
int LED1 = 3;
String data = "";
void setup(void){
 Serial.begin(115200);
 radio.begin();
 radio.openReadingPipe(1,pipe);
 radio.startListening();
 pinMode(LED1, OUTPUT);}

void loop(void){
 if (radio.available()){
   bool done = false;    
   while (!done){
     done = radio.read(msg, 1);
          
     if(msg[0] != '%'){
     data += msg[0];

     }
     else{
      Serial.println(data); 
     
      data = "";
   
      }
     
 }
 }}
