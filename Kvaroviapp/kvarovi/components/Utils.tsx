import {Animated, Dimensions} from "react-native"
import { keyword } from "./globals";
import { ComponentClass, FC } from "react";
import React from "react";

const figmaWidth = 360;
const figmaHeight = 800;
export function figmaToDeviceRatio(valueFigma: number,axis: "x" | "y"): number{
  const windowWidth = Dimensions.get('window').width;
  const windowHeight = Dimensions.get('window').height;
  let wRatio = windowWidth  / figmaWidth;
  let hRatio = windowHeight / figmaHeight;
  if(axis == "x") return valueFigma * wRatio;
  if(axis == "y") return valueFigma * hRatio;
  
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
