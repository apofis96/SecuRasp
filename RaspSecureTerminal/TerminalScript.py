from pad4pi import rpi_gpio
import sched, time, atexit, requests
import RPi.GPIO as GPIO
from RPLCD.i2c import CharLCD
from datetime import datetime

bcmBuzzer = 10
bcpRelay = 24
inputKeys = ''
screen = None
isWaiting = False
isScreanBusy = False
accessUrl = '127.0.0.1'
timeKeeper = None
lastModifyDate = datetime.now()

def main():
  print('Starting...')
  global timeKeeper
  atexit.register(exit_handler)
  configure()
  starBuzz()
  timeKeeper = sched.scheduler(time.time, time.sleep)
  timeKeeper.enter(1, 1, writeTime, (timeKeeper,))
  timeKeeper.run()

def writeTime(timeKeeper):
  global isScreanBusy
  if isScreanBusy:
    timeKeeper.enter(1, 1, writeTime, (timeKeeper,))
    return
  isScreanBusy = True
  dateNow = datetime.now()
  week = dateNow.weekday()
  month = dateNow.month
  tmpPos = screen.cursor_pos
  screen.cursor_pos=(0, 0)
  if week == 0:
    screen.write(0xA8)
    screen.write(0xBD)
  elif week == 1:
    screen.write(0x42)
    screen.write(0xBF)
  elif week == 2:
    screen.write_string('Cp')
  elif week == 3:
    screen.write(0xAB)
    screen.write(0xBF)
  elif week == 4:
    screen.write(0xA8)
    screen.write(0xBF)
  elif week == 5:
    screen.write(0x43)
    screen.write(0xB2)
  else:
    screen.write_string('Bc')

  screen.write_string(' ' + str(dateNow.day) + ' ')

  if month == 1:
    screen.write_string('\xB1\xBD\xB3')
  elif month == 2:
    screen.write_string('\xAA\x65\xB3')
  elif month == 3:
    screen.write_string('Map')
  elif month == 4:
    screen.write_string('A\xBEp')
  elif month == 5:
    screen.write_string('Ma\xB9')
  elif month == 6:
    screen.write_string('\xA5\xC6\xBD')
  elif month == 7:
    screen.write_string('\xA5\xC6\xBB')
  elif month == 8:
    screen.write_string('A\xB3')
    screen.write(0xB4)
  elif month == 9:
    screen.write_string('Ce\xBD')
  elif month == 10:
    screen.write_string('O\xBA\xBF')
  elif month == 11:
    screen.write_string('Ho\xC7')
  else:
    screen.write_string('\xE0e\xBA')
  screen.write_string(' ')
  screen.cursor_pos = (0,9)
  screen.write_string(' '+ dateNow.strftime("%H:%M"))
  screen.cursor_pos = tmpPos
  timeKeeper.enter(30, 1, writeTime, (timeKeeper,))
  global inputKeys
  if (dateNow-lastModifyDate).total_seconds() > 90 and inputKeys !='':
    inputKeys = ''
    clearCode()
    GPIO.output(bcmBuzzer, GPIO.HIGH)
    time.sleep(0.005)
    GPIO.output(bcmBuzzer, GPIO.LOW)
  isScreanBusy = False

def exit_handler():
  print('Stoping...')
  GPIO.cleanup()
  clearCode()
  screen.write_string('Oc\xBFa\xBDo\xB3\xBBe\xBD\xBDo')
  screen.cursor_mode = 'hide'

def configure():
  global screen
  GPIO.setmode(GPIO.BCM)
  GPIO.setup(bcpRelay, GPIO.OUT)
  GPIO.setup(bcmBuzzer, GPIO.OUT)
  GPIO.output(bcmBuzzer, GPIO.LOW)
  GPIO.output(bcpRelay, GPIO.LOW)

  screen = CharLCD(i2c_expander='PCF8574', address=0x27, port=1,
    cols=16, rows=2, dotsize=8,
    charmap='A02', auto_linebreaks=False, backlight_enabled=True)
  screen.clear()
  threeDots = (0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x15)
  checkMark = (0x00, 0x00, 0x01, 0x02, 0x14, 0x08, 0x00, 0x00)
  crossMark = (0x00, 0x00, 0x11, 0x0A, 0x04, 0x0A, 0x11, 0x00)
  sandClock = (0x00, 0x1F, 0x11, 0x0A, 0x04, 0x0A, 0x11, 0x1F)
  screen.create_char(0, threeDots)
  screen.create_char(1, checkMark)
  screen.create_char(2, crossMark)
  screen.create_char(3, sandClock)
  screen.cursor_mode = 'blink'
  screen.write_string(' \x03 \x03 \x03 \x03\x03 \x03 \x03 \x03')
  screen.cursor_pos=(1, 0)

  KEYPAD = [
    ["1","2","3"],
    ["4","5","6"],
    ["7","8","9"],
    ["*","0","#"]
  ]
  ROW_PINS = [14,15,17,18]
  COL_PINS = [27,22,23]
  factory = rpi_gpio.KeypadFactory() 
  keypad = factory.create_keypad(keypad=KEYPAD, row_pins=ROW_PINS, col_pins=COL_PINS)
  keypad.registerKeyPressHandler(processKey)

def processKey(key):
  global isScreanBusy
  if isWaiting:
    return
  if isScreanBusy:
    time.sleep(0.1)
    processKey(key)
    return
  global inputKeys
  isScreanBusy = True
  GPIO.output(bcmBuzzer, GPIO.HIGH)
  time.sleep(0.005)
  GPIO.output(bcmBuzzer, GPIO.LOW)
  if key == '#':
    checkAccess(inputKeys)
    inputKeys = ''
    clearCode()
  elif key == '*':
    inputKeys = inputKeys[:-1]
    clearCode()
  elif len(inputKeys) > 19:
    isScreanBusy = False
    return
  else:
    inputKeys += key
  writeCode(inputKeys)

def checkAccess(keys):
  global isWaiting
  isWaiting = True
  clearCode()
  screen.cursor_mode = 'hide'
  screen.cursor_pos=(1, 3)
  screen.write(0xA8)
  screen.write_string('po\xB3ep\xBAa \x03')
  try:
    x = requests.get(accessUrl + '/' + keys)
    if x.status_code == 200:
      clearCode()
      screen.cursor_mode = 'hide'
      screen.cursor_pos=(1, 3)
      screen.write(0xA8)
      screen.write(0x70)
      screen.write(0xB8)
      screen.write_string('\xBD\xC7\xBFo \x01')
      openDoor()
    else:
      clearCode()
      screen.cursor_mode = 'hide'
      screen.cursor_pos=(1, 3)
      screen.write_string('O\xBF\xBAa\xB7a\xBDo \x02')
      GPIO.output(bcmBuzzer, GPIO.HIGH)
      time.sleep(1)
      GPIO.output(bcmBuzzer, GPIO.LOW)
      time.sleep(1)
  finally:
    isWaiting = False
    clearCode()

def writeCode(code):
  global isScreanBusy
  global lastModifyDate
  lastModifyDate = datetime.now()
  screen.cursor_pos=(1, 0)
  if len(code) == 20:
    screen.cursor_mode = 'hide'
  if len(code) > 14:
    screen.write(0x00)
    screen.write_string(code[len(code)-14:])
  else:
    screen.write_string(code)
  isScreanBusy = False

def clearCode():
  screen.cursor_pos=(1, 0)
  screen.write_string('               ')
  screen.cursor_pos=(1, 0)
  screen.cursor_mode = 'blink'

def starBuzz():
  for x in range(3):
    time.sleep(0.25)
    GPIO.output(bcmBuzzer, GPIO.HIGH)
    time.sleep(0.05)
    GPIO.output(bcmBuzzer, GPIO.LOW)

def openDoor():
  GPIO.output(bcpRelay, GPIO.HIGH)
  for x in range(20):
    GPIO.output(bcmBuzzer, GPIO.HIGH)
    time.sleep(0.05)
    GPIO.output(bcmBuzzer, GPIO.LOW)
    time.sleep(0.2)
  GPIO.output(bcpRelay, GPIO.LOW)

main()
