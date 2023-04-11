mergeInto(LibraryManager.library, {
  RouletteOrbLoaded: function () {
    ReactUnityWebGL.RouletteOrbLoaded();
  },
  CoinsUpdated: function (coins) {
    ReactUnityWebGL.CoinsUpdated(coins);
  }
});