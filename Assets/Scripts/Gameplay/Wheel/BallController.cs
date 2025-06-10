using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using Events;
using Events.EventTypes;
using Events.EventTypes.Audio;
using SubSystems.Audio;

namespace Gameplay.Wheel
{
    /// <summary>
    /// Controls roulette ball behavior including idle spin, targeted spin,
    /// smooth diamond approach, and animated pocket jump with final event raise.
    /// </summary>
    public class BallController : MonoBehaviour
    {
        [SerializeField] private AudioSource spinLoopAudioSource;
        // References and state
        private Transform _ball;
        private float _spinSpeed;
        private Vector3 _initPosition;

        private Transform _targetPocket;
        private Transform _targetDiamond;

        private List<Transform> _jumpPoints;
        private List<Transform> _diamonds;

        private Transform _wheelTransform;

        private float _currentAngle;
        private bool _isSpinning;
        private int _determinedNumber;
        private int _slotCount;
        private IEnumerator _idleRoutineCo;

        /// <summary>
        /// Initializes the ball controller with required transforms and values.
        /// </summary>
        public void Initialize(Transform ballTransform, Vector3 startPosition, float spinSpeedValue, Transform wheel, List<Transform> diamonds, List<Transform> jumpPointTransforms, int slotCount)
        {
            _ball = ballTransform;
            _initPosition = startPosition;
            _spinSpeed = spinSpeedValue;
            _wheelTransform = wheel;
            _targetDiamond = null;
            _currentAngle = 0f;
            _isSpinning = false;
            _jumpPoints = jumpPointTransforms;
            _diamonds = diamonds;
            _slotCount = slotCount;
        }

        /// <summary>
        /// Resets the ball to initial state and position.
        /// </summary>
        public void ResetBall()
        {
            _isSpinning = false;
            _ball.SetParent(null);
            _ball.position = _initPosition;
            _currentAngle = 0f;
        }

        /// <summary>
        /// Starts spinning the ball toward a specific pocket.
        /// </summary>
        public void SpinToTarget(Transform pocket,int number)
        {
            if (_isSpinning) return;
            if(_idleRoutineCo != null)
                StopCoroutine(_idleRoutineCo);

            ResetBall();
            _determinedNumber = number;
            _targetPocket = pocket;
            _isSpinning = true;
            _ball.SetParent(null);
            _ball.position = _initPosition;
            _currentAngle = 0f;

            StartCoroutine(SpinRoutine());
        }

        /// <summary>
        /// Starts idle spin coroutine.
        /// </summary>
        public void IdleRoutine()
        {
            if(_idleRoutineCo != null)
                StopCoroutine(_idleRoutineCo);

            ResetBall();
            _idleRoutineCo = IdleRoutineCo();
            StartCoroutine(_idleRoutineCo);
        }

        /// <summary>
        /// Coroutine that spins the ball continuously in idle state.
        /// </summary>
        private IEnumerator IdleRoutineCo()
        {
            while (true)
            {
                SpinAroundWheel();
                yield return null;
            }
        }

        /// <summary>
        /// Full spin behavior including delay, angle tracking, diamond transition,
        /// jump sequence, and result event.
        /// </summary>
        private IEnumerator SpinRoutine()
        {
            var elapsed = 0f;

            spinLoopAudioSource.enabled = true;
            // Delay before approaching the pocket
            while (elapsed < 3)
            {
                elapsed += Time.deltaTime;
                SpinAroundWheel();
                yield return null;
            }

            var targetPocketAngle = GetWheelLocalAngle(_targetPocket.position);
            var ballAngle = GetWheelLocalAngle(_ball.position);
            var thresholdAngle = 30f;

            // Spin until angle close to target
            while (true)
            {
                SpinAroundWheel();
                ballAngle -= _spinSpeed * Time.deltaTime;
                if (ballAngle <= 0f) ballAngle += 360f;

                targetPocketAngle = GetWheelLocalAngle(_targetPocket.position);
                var angleDifference = Mathf.Abs(Mathf.DeltaAngle(ballAngle, targetPocketAngle));

                if (angleDifference < thresholdAngle)
                    break;

                yield return null;
            }

            _targetDiamond = FindClosestDiamondToPocket(_targetPocket);

            var transitionElapsed = 0f;
            var isTransitioning = true;
            var targetHeight = 0.43f;
            var targetRadius = 4.74f;
            var startHeight = 0.41f;
            var startRadius = 5.58f;

            var transitionStartAngle = _currentAngle;
            var currentDifTarget = Mathf.Abs(Mathf.DeltaAngle(ballAngle, GetWheelLocalAngle(_targetDiamond.position)));
            var transitionTargetAngle = _currentAngle + currentDifTarget;
            var angularDistance = Mathf.Abs(transitionTargetAngle - transitionStartAngle);
            var transitionDuration = angularDistance / _spinSpeed;

            var appliedRotateAroundRadius = startRadius;
            var appliedRotateAroundHeight = startHeight;

            while (Vector3.Distance(_ball.position, _targetDiamond.position) > 0.2f)
            {
                _currentAngle += _spinSpeed * Time.deltaTime;
                if (_currentAngle >= 360f) _currentAngle -= 360f;

                if (isTransitioning)
                {
                    transitionElapsed += Time.deltaTime;
                    var t = Mathf.Clamp01(transitionElapsed / transitionDuration);
                    appliedRotateAroundRadius = Mathf.Lerp(startRadius, targetRadius, t);
                    appliedRotateAroundHeight = Mathf.Lerp(startHeight, targetHeight, t);
                    if (t >= 1f) isTransitioning = false;
                }

                var offset = new Vector3(
                    Mathf.Sin(Mathf.Deg2Rad * _currentAngle),
                    appliedRotateAroundHeight,
                    Mathf.Cos(Mathf.Deg2Rad * _currentAngle)
                ) * appliedRotateAroundRadius;

                _ball.position = _wheelTransform.position + offset;
                yield return null;
            }

            var path = GetRandomJumpPath(_targetPocket.position, 2, 4);
            var iteration = path.Count;

            spinLoopAudioSource.enabled = false;
            AudioEvents.RequestSound(SoundType.BallBounce);
            foreach (var point in path)
            {
                yield return JumpToPoint(point.position, iteration--, path.Count);
            }

            _ball.SetParent(_targetPocket);
            yield return JumpToPoint(Vector3.zero, 1, 2, true);
            _ball.localPosition = Vector3.zero;
            _isSpinning = false;

            EventBus<BallStoppedEvent>.Raise(new BallStoppedEvent
            {
                ResultNumber = _determinedNumber,
                SlotNumberCount = _slotCount
            });
        }

        /// <summary>
        /// Spins the ball around the wheel with fixed height and radius.
        /// </summary>
        private void SpinAroundWheel()
        {
            _currentAngle += _spinSpeed * Time.deltaTime;
            if (_currentAngle >= 360f) _currentAngle -= 360f;

            var offset = new Vector3(
                Mathf.Sin(Mathf.Deg2Rad * _currentAngle),
                0.41f,
                Mathf.Cos(Mathf.Deg2Rad * _currentAngle)
            ) * 5.58f;

            _ball.position = _wheelTransform.position + offset;
        }

        /// <summary>
        /// Converts world position to angle relative to wheel center.
        /// </summary>
        private float GetWheelLocalAngle(Vector3 worldPos)
        {
            var direction = worldPos - _wheelTransform.position;
            var angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
            return (angle + 360f) % 360f;
        }

        /// <summary>
        /// Finds the closest diamond to the given pocket.
        /// </summary>
        private Transform FindClosestDiamondToPocket(Transform pocket)
        {
            Transform closestDiamond = null;
            var closestDist = float.MaxValue;

            foreach (var diamond in _diamonds)
            {
                var dist = Vector3.Distance(diamond.position, pocket.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestDiamond = diamond;
                }
            }

            return closestDiamond;
        }

        /// <summary>
        /// Returns a list of random jump points near the target pocket.
        /// </summary>
        private List<Transform> GetRandomJumpPath(Vector3 end, int minPoints, int maxPoints)
        {
            var pathPoints = new List<Transform>();
            var candidates = new List<Transform>();
            var maxJumpPointRadius = 1f;

            foreach (var jp in _jumpPoints)
            {
                var distToTarget = Vector3.Distance(jp.position, end);
                if (distToTarget <= maxJumpPointRadius)
                    candidates.Add(jp);
            }

            var pointCount = Mathf.Min(Random.Range(minPoints, maxPoints + 1), candidates.Count);

            for (var i = 0; i < pointCount; i++)
            {
                var randomIndex = Random.Range(0, candidates.Count);
                pathPoints.Add(candidates[randomIndex]);
                candidates.RemoveAt(randomIndex);
            }

            return pathPoints;
        }

        /// <summary>
        /// Animates a parabolic jump from current position to target.
        /// </summary>
        private IEnumerator JumpToPoint(Vector3 targetPoint, int currentIteration, int maxIteration, bool isLocal = false)
        {
            var startPoint = isLocal ? _ball.localPosition : _ball.position;
            var elapsedTime = 0f;
            var jumpDuration = Random.Range(.3f, 1);
            var heigtDelta = (float)(currentIteration - 1) / (maxIteration - 1);
            var dynamicMax = Mathf.Lerp(.6f, 2f, heigtDelta);
            var jumpHeight = Random.Range(.5f, dynamicMax);

            while (elapsedTime < jumpDuration)
            {
                var t = elapsedTime / jumpDuration;
                var height = 4 * jumpHeight * t * (1 - t);
                var horizontalPos = Vector3.Lerp(startPoint, targetPoint, t);

                if (!isLocal)
                    _ball.position = new Vector3(horizontalPos.x, horizontalPos.y + height, horizontalPos.z);
                else
                    _ball.localPosition = new Vector3(horizontalPos.x, horizontalPos.y + height, horizontalPos.z);

                elapsedTime += Time.deltaTime;
                yield return null;
            }
            AudioEvents.RequestSound(SoundType.BallBounce);
            _ball.position = targetPoint;
        }
    }
}
