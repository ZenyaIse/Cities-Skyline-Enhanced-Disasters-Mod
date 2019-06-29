# More Natural Disasters mod

Mod for Cities: Skylines

With this mod natural disasters occur in more natural way, depending on weather, season, or time passed since the last disaster.


## Forest Fire

Features
* Do not occur during rain.
* Probability gradually increases during dry weather.

Unlocks
* From the beginning (can occur only outside of your unlocked areas)
* From Milestone 3 (can occur everywere, both inside and outside of your unlocked areas)

Default settings
* Warmup period: 180 days without rain
* Peak probability: 10 times/year


## Thunderstorm

Features
* Occurs mainly in thunderstorm season.
* Probability is higher during rain.

Unlocks
* From the beginning (can occur only outside of your unlocked areas)
* From Milestone 3 (can occur everywere, both inside and outside of your unlocked areas)

Default settings
* Thunderstorm season peak: July
* Probability in the season peak: 2 times/year
* Probability during rain: x2


## Sinkhole

Features
* Occurs during a long rainfall and for a short time after the rain (like landslides).
* The longer and heavier the rainfall, the higher the probability. Imagine an underground reservour which is filling up during rain and emptying after rain stops.

Unlocks
* From Milestone 4 (can occur only inside your unlocked areas)

Default settings
* Maximum probability after a long heavy rainfall: 1.5 times/year
* Underground reservour capacity: 50 days of heavy rainfall


## Tornado

Features
* Occurs mainly in tornado season.
* Do not occur during fog (when the wind is weak).
* Small tornadoes occur more often than big ones.

Unlocks
* From Milestone 5 (can occur only inside your unlocked areas)

Default settings
* Tornado season peak: May
* Probability in the season peak: 2 times/year


## Tsunami

Features
* Probability slowly increases with time.
* Cannot occur too often (there is long calm period after each tsunami).
* Small tsunamies occur more often than big ones.

Unlocks
* From Milestone 5

Default settings
* Maximum probability after a long time: 1 times/year
* Charge period during wich probability increases: 5 years
* Calm period after tsunami: 1 year (automatically set as 1/5 of the charge period)


## Earthquake

Features
* Probability slowly increases with time.
* Cannot occur too often (there is long calm period after each earthquake), excluding aftershocks.
* Small earthquakes occur more often than big ones (Gutenberg–Richter law distribution for earthquakes is used).
* One or several aftershocks can occur after a big earthquake (may take several months to calm down).
* Aftershocks strike the same place as the main earthquake.
* Every following aftershock is weaker than the previous one.
* No cracks appear (too destructive).

Unlocks
* From Milestone 6 (can occur only inside your unlocked areas)

Default settings
* Maximum probability after a long time: 1 times/year
* Charge period during wich probability increases: 3 years
* Calm period after earthquake: 1 year (automatically set as 1/3 of the charge period)
* Aftershocks enabled


## Meteor Strike

Features
* There are three meteoroid streams periodically approaching the Earth.
  * Meteoroid stream 1: period 9 years, maximum size 100
  * Meteoroid stream 2: period 5 years, maximum size 50
  * Meteoroid stream 3: period 2 years, maximum size 30
* When a meteoroid stream approaches the Earth, there is a chance of a meteor strike.

Unlocks
* From Milestone 6 (can occur only inside your unlocked areas)

Default settings
* Peak probability when a meteoroid stream is approaching: 5 times/year (per meteoroid stream). Cumulative.


## Disasters info panel

Shows current probabilities and maximum intesity values for all disasters. Some detailed info can be seen in toolips by mouseover.

Also contains the Emergency Button (see below).

Show / hide the disasters info panel by clicking the "Lightning" button at the top-left of the screen, or use the key shortcut Shift+D.


## Emergency Button

One of the most annoying thing about disasters is that they soullessly destroy the world you wholeheartedly created. To save your most valuable creation from destruction, just press the Emergency Button at the bottom of the disasters info panel * this stops all currently occuring disasters, including falling meteors and approaching tsunami waves.

In the current version, there are no restrictions how many times you can press the Emergency Button.


## Logging

The mod logs out information about all occured disasters into csv file in the CS data directory (C:\Users\%Username%\AppData\Colossal Order\Cities_Skylines).
* date
* name
* intensity
* reason (mod or vanilla)

Can be turned on/off in the mod options.


## Minor features
* Maximum intensity for all disasters is set to the minimum (10) at the beginning of the game and gradually increases up to the maximum (100) until the city population reaches 20000.
* Duration of small intensity thunderstorms is decreased.
* Meteoroid stream period and phase (next approaching time) are randomized at the beginning of the game.
