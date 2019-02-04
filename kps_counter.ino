/*
 * kps_counter.ino
 * 
 * MIT License
 * 
 * Copyright (c) 2019 Michael Gallagher
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

#include <SevSeg.h>

// drives the seven segment display
SevSeg sevseg;

// buffer for receiving from serial connection
// one for each digit, one for \0
#define MAX_CHARS 5
char received_chars[MAX_CHARS];

// holding the unserialized data
int num_from_serial = 0;

// Arduino entry
void setup(){
  // for setting up the seven-segment display
  byte numDigits = 4;
  byte digitPins[] = {10, 11, 12, 13};
  byte segmentPins[] = {9, 2, 3, 5, 6, 8, 7, 4};
  byte hardwareConfig = COMMON_CATHODE; // See README.md for options
  sevseg.begin(hardwareConfig, numDigits, digitPins, segmentPins);
  sevseg.setBrightness(100);
  sevseg.setNumber(0);

  // set up the serial connection
  Serial.begin(9600);
  Serial.println("<Arduino is ready>");
}

// Arduino loop
void loop(){
  // check if new data is available
  if (receive_from_serial()) {
    // deserialize and queue for display
    num_from_serial = atoi(received_chars);
    sevseg.setNumber(num_from_serial);
  }
  sevseg.refreshDisplay();
}

// tries to receive a string from serial connection
// returns true if data has been received, false otherwise
// adapted from code at http://forum.arduino.cc/index.php?topic=396450.0
bool receive_from_serial() {
  static byte ndx = 0; // array offset
  char end_marker = '\n'; // end of serial data token
  char rc;
  bool new_data = false; // return value

  if (Serial.available() > 0) {
    rc = Serial.read();

    // if we have a new character add it to the buffer and increment the offset
    if (rc != end_marker) {
      received_chars[ndx] = rc;
      ndx++;
      if (ndx >= MAX_CHARS) {
        // failsafe to prevent buffer overflow
        ndx = MAX_CHARS - 1;
      }
    } else {
      // terminate with null and set our flag
      received_chars[ndx] = '\0';
      ndx = 0;
      new_data = true;
    }
  }

  return new_data;
}
