
import TextInputHandles from "@gorhom/bottom-sheet";
import React, { memo, useCallback, forwardRef, useEffect } from 'react';
import { TextInput, TextInputProps } from 'react-native-paper';
import { useBottomSheetInternal,  } from '@gorhom/bottom-sheet';

interface BottomSheetTextInputComponentProps extends TextInputProps {
  onFocus?: (args: any) => void;
  onBlur?: (args: any) => void;
}

const BottomSheetTextInputComponent = forwardRef<React.Ref<any>, BottomSheetTextInputComponentProps>(
  ({ onFocus, onBlur, ...rest }, ref) => {
    const { shouldHandleKeyboardEvents } = useBottomSheetInternal();

    useEffect(() => {
      return () => {
        shouldHandleKeyboardEvents.value = false;
      };
    }, [shouldHandleKeyboardEvents]);

    const handleOnFocus = useCallback(
      (args) => {
        shouldHandleKeyboardEvents.value = true;
        if (onFocus) {
          onFocus(args);
        }
      },
      [onFocus, shouldHandleKeyboardEvents]
    );

    const handleOnBlur = useCallback(
      (args) => {
        shouldHandleKeyboardEvents.value = false;
        if (onBlur) {
          onBlur(args);
        }
      },
      [onBlur, shouldHandleKeyboardEvents]
    );

    return (
      <TextInput
        ref={ref as any}
        onFocus={handleOnFocus}
        onBlur={handleOnBlur}
        {...rest}
      />
    );
  }
);

const MemoizedBottomSheetTextInputComponent = memo(BottomSheetTextInputComponent);

export default MemoizedBottomSheetTextInputComponent;
