import {Animated, DimensionValue, Dimensions} from "react-native"
import { announcementData, keyword } from "./globals";
import { ComponentClass, FC } from "react";
import React from "react";

export const figmaWidth = 360;
export const figmaHeight = 800;


export function figmaToDeviceRatio(valueFigma: number,axis: "x" | "y"): number{
  const windowWidth = Dimensions.get('window').width;
  const windowHeight = Dimensions.get('window').height;
  let wRatio = windowWidth  / figmaWidth;
  let hRatio = windowHeight / figmaHeight;
  if(axis == "x") return valueFigma * wRatio;
  if(axis == "y") return valueFigma * hRatio;
  
}

export function calculatePercentage(valueFigma,axis: "x" | "y") : DimensionValue{
  var value = figmaToDeviceRatio(valueFigma,axis);
  var main;
  if(axis == 'x') main = figmaWidth;
  else main = figmaHeight;
  var percentage = ((main - value) / main ) * 100;
  var dimValue: DimensionValue = `${100 - percentage}%`
  console.log(dimValue,main,value)
  return dimValue
}
const wrapFunctionComponent = <TProps,>(
  Component: FC<TProps>
): ComponentClass<TProps> =>
  class extends React.Component<TProps> {
    constructor(props: TProps) {
      super(props)
    }

    render() {
      return <Component {...this.props} />
    }
  }

export const createAnimatedFunctionComponent = <TProps,>(
  Component: FC<TProps>
) => Animated.createAnimatedComponent(wrapFunctionComponent(Component))

